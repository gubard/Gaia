using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Gaia.Helpers;
using Gaia.Models;

namespace Gaia.Services;

public interface IFtpClientService : IDisposable, IAsyncDisposable
{
    ConfiguredValueTaskAwaitable<FtpItem> GetItemAsync(string path, CancellationToken ct);
    ConfiguredValueTaskAwaitable<bool> IsExistsAsync(string path, CancellationToken ct);
    ConfiguredValueTaskAwaitable UploadItemAsync(string path, Stream data, CancellationToken ct);
    ConfiguredValueTaskAwaitable DownloadItemAsync(string path, Stream data, CancellationToken ct);
    ConfiguredValueTaskAwaitable DeleteItemAsync(string path, CancellationToken ct);
    ConfiguredValueTaskAwaitable<FtpItem> GetCurrenDirectoryAsync(CancellationToken ct);

    ConfiguredValueTaskAwaitable<Memory<FtpItem>> GetListItemAsync(
        string path,
        CancellationToken ct
    );
}

public sealed class FtpClientService : IFtpClientService
{
    public FtpClientService(
        TcpClient tcpClient,
        NetworkStream stream,
        StreamWriter streamWriter,
        StreamReader streamReader
    )
    {
        _tcpClient = tcpClient;
        _streamWriter = streamWriter;
        _streamReader = streamReader;
        _stream = stream;
    }

    public ConfiguredValueTaskAwaitable<bool> IsExistsAsync(string path, CancellationToken ct)
    {
        return IsExistsCore(path, ct).ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable UploadItemAsync(
        string path,
        Stream data,
        CancellationToken ct
    )
    {
        return UploadItemCore(path, data, ct).ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable DownloadItemAsync(
        string path,
        Stream data,
        CancellationToken ct
    )
    {
        return DownloadItemCore(path, data, ct).ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable DeleteItemAsync(string path, CancellationToken ct)
    {
        return DeleteItemCore(path, ct).ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable<FtpItem> GetCurrenDirectoryAsync(CancellationToken ct)
    {
        return GetCurrenDirectoryCore(ct).ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable<Memory<FtpItem>> GetListItemAsync(
        string path,
        CancellationToken ct
    )
    {
        return GetListItemCore(path, ct).ConfigureAwait(false);
    }

    public static ConfiguredValueTaskAwaitable<FtpClientService> CreateAsync(
        string host,
        int port,
        string username,
        string password,
        CancellationToken ct
    )
    {
        return CreateCore(host, port, username, password, ct).ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable<FtpItem> GetItemAsync(string path, CancellationToken ct)
    {
        return GetItemCore(path, ct).ConfigureAwait(false);
    }

    private readonly TcpClient _tcpClient;
    private readonly NetworkStream _stream;
    private readonly StreamWriter _streamWriter;
    private readonly StreamReader _streamReader;

    private async ValueTask<bool> IsExistsCore(string path, CancellationToken ct)
    {
        var dir = Path.GetDirectoryName(path).ThrowIfNull();
        await ExecuteAsync($"CWD \"{dir}\"", FtpStatusCode.RequestedFileActionOkay, ct);
        await _streamWriter.WriteLineAsync($"SIZE \"{Path.GetFileName(path)}\"");
        var response = await _streamReader.ReadLineAsync(ct);

        return response.ThrowIfNull().StartsWith("213");
    }

    private async ValueTask DownloadItemCore(string path, Stream data, CancellationToken ct)
    {
        var dir = Path.GetDirectoryName(path).ThrowIfNull();
        await ExecuteAsync($"CWD \"{dir}\"", FtpStatusCode.RequestedFileActionOkay, ct);
        await ExecuteAsync("TYPE I", FtpStatusCode.CommandOkay, ct);
        var parameters = await GetPasvAsync(ct);

        using (var dataClient = new TcpClient(parameters.Host, parameters.Port))
        {
            await ExecuteAsync(
                $"RETR \"{Path.GetFileName(path)}\"",
                FtpStatusCode.OpeningAsciiModeDataConnection,
                ct
            );

            await using var dataStream = dataClient.GetStream();
            await dataStream.CopyToAsync(data, ct);
        }

        await ReadExpectedStatusAsync(FtpStatusCode.ClosingDataConnectionSuccessful, ct);
    }

    private async ValueTask<FtpItem> GetItemCore(string path, CancellationToken ct)
    {
        var dir = Path.GetDirectoryName(path).ThrowIfNull();
        await ExecuteAsync($"CWD \"{dir}\"", FtpStatusCode.RequestedFileActionOkay, ct);

        var response = await ExecuteAsync(
            $"MLST \"{Path.GetFileName(path)}\"",
            FtpStatusCode.RequestedFileActionOkay,
            ct
        );

        return ParseFtpItem(response.Span, path);
    }

    private async ValueTask UploadItemCore(string path, Stream data, CancellationToken ct)
    {
        var dir = Path.GetDirectoryName(path).ThrowIfNull();
        await ExecuteAsync($"CWD \"{dir}\"", FtpStatusCode.RequestedFileActionOkay, ct);
        await ExecuteAsync("TYPE I", FtpStatusCode.CommandOkay, ct);
        var parameters = await GetPasvAsync(ct);

        using (var dataClient = new TcpClient(parameters.Host, parameters.Port))
        {
            await ExecuteAsync(
                $"STOR \"{Path.GetFileName(path)}\"",
                FtpStatusCode.OpeningAsciiModeDataConnection,
                ct
            );

            await using var dataStream = dataClient.GetStream();
            await data.CopyToAsync(dataStream, ct);
        }

        await ReadExpectedStatusAsync(FtpStatusCode.ClosingDataConnectionSuccessful, ct);
    }

    private async ValueTask DeleteItemCore(string path, CancellationToken ct)
    {
        var item = await GetItemAsync(path, ct);
        await DeleteItemAsync(item, ct);
    }

    private async ValueTask DeleteItemAsync(FtpItem item, CancellationToken ct)
    {
        var dir = Path.GetDirectoryName(item.Path).ThrowIfNull();

        if (item.Type == FtpItemType.File)
        {
            await ExecuteAsync($"CWD \"{dir}\"", FtpStatusCode.RequestedFileActionOkay, ct);

            await ExecuteAsync(
                $"DELE \"{Path.GetFileName(item.Path)}\"",
                FtpStatusCode.RequestedFileActionOkay,
                ct
            );
        }
        else
        {
            var items = await GetListItemCore(item.Path, ct);
            var array = items.ToArray();

            foreach (var i in array)
            {
                await DeleteItemAsync(i, ct);
            }

            await ExecuteAsync($"CWD \"{dir}\"", FtpStatusCode.RequestedFileActionOkay, ct);

            await ExecuteAsync(
                $"RMD \"{Path.GetFileName(item.Path)}\"",
                FtpStatusCode.RequestedFileActionOkay,
                ct
            );
        }
    }

    private async ValueTask<Memory<FtpItem>> GetListItemCore(string path, CancellationToken ct)
    {
        await ExecuteAsync($"CWD \"{path}\"", FtpStatusCode.RequestedFileActionOkay, ct);
        var parameters = await GetPasvAsync(ct);
        var fileEntries = new List<FtpItem>();

        using (var dataClient = new TcpClient(parameters.Host, parameters.Port))
        {
            await ExecuteAsync("MLSD", FtpStatusCode.OpeningAsciiModeDataConnection, ct);
            await using var dataStream = dataClient.GetStream();
            using var dataReader = new StreamReader(dataStream);

            while (await dataReader.ReadLineAsync(ct) is { } entry)
            {
                fileEntries.Add(ParseFtpItem(entry, path));
            }
        }

        await ReadExpectedStatusAsync(FtpStatusCode.ClosingDataConnectionSuccessful, ct);

        return fileEntries.ToArray();
    }

    private async ValueTask<(string Host, int Port)> GetPasvAsync(CancellationToken ct)
    {
        var response = await ExecuteAsync("PASV", FtpStatusCode.PasvSuccessful, ct);
        var valuesStartIndex = response.Span.IndexOf('(');
        var valuesEndIndex = response.Span.LastIndexOf(')');
        var valuesStr = response.Slice(valuesStartIndex + 1, valuesEndIndex - valuesStartIndex - 1);
        var values = valuesStr.Span.Split(',');
        var parameters = ParsePasvResponse(values);

        return parameters;
    }

    private FtpItem ParseFtpItem(ReadOnlySpan<char> entry, string path)
    {
        var spaceIndex = entry.IndexOf(' ');
        var entryPath = Path.Combine(path, entry.Slice(spaceIndex + 1).ToString());

        var type = entry.Contains("type=dir", StringComparison.Ordinal)
            ? FtpItemType.Directory
            : FtpItemType.File;

        return new(entryPath, type);
    }

    private (string Host, int Port) ParsePasvResponse(
        MemoryExtensions.SpanSplitEnumerator<char> values
    )
    {
        Span<Range> ranges = stackalloc Range[6];
        var index = 0;

        foreach (var value in values)
        {
            ranges[index] = value;
            index++;
        }

        return (
            $"{values.Source[ranges[0]]}.{values.Source[ranges[1]]}.{values.Source[ranges[2]]}.{values.Source[ranges[3]]}",
            int.Parse(values.Source[ranges[4]]) * 256 + int.Parse(values.Source[ranges[5]])
        );
    }

    private async ValueTask<FtpItem> GetCurrenDirectoryCore(CancellationToken ct)
    {
        var response = await ExecuteAsync("PWD", FtpStatusCode.PathNameCreated, ct);
        var firstQuote = response.Span.IndexOf('\"');
        var lastQuote = response.Span.LastIndexOf('\"');
        var path = response.Slice(firstQuote + 1, lastQuote - firstQuote - 1);
        var item = await GetItemAsync(path.Span.ToString(), ct);

        return item;
    }

    private static async ValueTask<FtpClientService> CreateCore(
        string host,
        int port,
        string username,
        string password,
        CancellationToken ct
    )
    {
        var controlClient = new TcpClient(host, port);
        var stream = controlClient.GetStream();
        var writer = new StreamWriter(stream) { AutoFlush = true };
        var reader = new StreamReader(stream);
        await ReadExpectedStatusAsync(reader, FtpStatusCode.ServiceReady, ct);
        await writer.WriteLineAsync($"USER {username}");
        await ReadExpectedStatusAsync(reader, FtpStatusCode.UserNameOkay, ct);
        await writer.WriteLineAsync($"PASS {password}");
        await ReadExpectedStatusAsync(reader, FtpStatusCode.UserLoggedIn, ct);

        return new(controlClient, stream, writer, reader);
    }

    private async ValueTask<ReadOnlyMemory<char>> ExecuteAsync(
        string command,
        FtpStatusCode expectedStatus,
        CancellationToken ct
    )
    {
        await _streamWriter.WriteLineAsync(command);

        return await ReadExpectedStatusAsync(expectedStatus, ct);
    }

    private ValueTask<ReadOnlyMemory<char>> ReadExpectedStatusAsync(
        FtpStatusCode expectedStatus,
        CancellationToken ct
    )
    {
        return ReadExpectedStatusAsync(_streamReader, expectedStatus, ct);
    }

    private static async ValueTask<ReadOnlyMemory<char>> ReadExpectedStatusAsync(
        StreamReader reader,
        FtpStatusCode expectedStatus,
        CancellationToken ct
    )
    {
        var str = await reader.ReadLineAsync(ct);
        var strSpan = str.ThrowIfNull().AsSpan();
        var index = strSpan.IndexOf(' ');
        CheckStatus(index == -1 ? strSpan : strSpan.Slice(0, index), expectedStatus);

        return strSpan.ToArray();
    }

    private static void CheckStatus(ReadOnlySpan<char> str, FtpStatusCode expectedStatus)
    {
        var status = str.ToFtpStatusCode();

        if (status == expectedStatus)
        {
            return;
        }

        throw new ExpectedFtpStatusCodeException(status);
    }

    public void Dispose()
    {
        _tcpClient.Dispose();
        _stream.Dispose();
        _streamWriter.Dispose();
        _streamReader.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        _tcpClient.Dispose();
        await _stream.DisposeAsync();
        await _streamWriter.DisposeAsync();
        _streamReader.Dispose();
    }
}

public enum FtpStatusCode
{
    CommandOkay = 200,
    ServiceReady = 220,
    UserNameOkay = 331,
    UserLoggedIn = 230,
    NotLoggedIn = 530,
    RequestedFileActionOkay = 250,
    PathNameCreated = 257,
    RequestedActionNotTaken = 550,
    DataConnectionAlreadyOpen = 125,
    FileStatusOkay = 150,
    ClosingDataConnectionSuccessful = 226,
    NotOpenDataConnection = 425,
    ConnectionClosedAborted = 426,
    PasvSuccessful = 227,
    OpeningAsciiModeDataConnection = 150,
}
