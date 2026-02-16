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
        return GetListItemCore(path, false, ct).ConfigureAwait(false);
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

    public static ReadOnlyMemory<string> Months = new[]
    {
        "Jan",
        "Feb",
        "Mar",
        "Apr",
        "May",
        "Jun",
        "Jul",
        "Aug",
        "Sep",
        "Oct",
        "Nov",
        "Dec",
    };

    private readonly TcpClient _tcpClient;
    private readonly NetworkStream _stream;
    private readonly StreamWriter _streamWriter;
    private readonly StreamReader _streamReader;

    private FtpClientService(
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

    private async ValueTask<bool> IsExistsCore(string path, CancellationToken ct)
    {
        await _streamWriter.WriteLineAsync($"SIZE \"{path}\"");
        var response = await _streamReader.ReadLineAsync(ct);

        return response.ThrowIfNull().StartsWith("213");
    }

    private async ValueTask DownloadItemCore(string path, Stream data, CancellationToken ct)
    {
        await ExecuteAsync("TYPE I", 200, ct);
        var parameters = await GetPasvAsync(ct);

        using (var dataClient = new TcpClient(parameters.Host, parameters.Port))
        {
            await ExecuteAsync($"RETR {path}", 150, ct);
            await using var dataStream = dataClient.GetStream();
            await dataStream.CopyToAsync(data, ct);
        }

        await ReadExpectedStatusAsync(226, ct);
    }

    private async ValueTask<FtpItem> GetItemCore(string path, CancellationToken ct)
    {
        var items = await GetListItemCore(path, true, ct);

        return items.ToArray().Single(i => i.Path == path);
    }

    private async ValueTask UploadItemCore(string path, Stream data, CancellationToken ct)
    {
        await ExecuteAsync("TYPE I", 200, ct);
        var parameters = await GetPasvAsync(ct);

        using (var dataClient = new TcpClient(parameters.Host, parameters.Port))
        {
            await ExecuteAsync($"STOR {path}", 150, ct);
            await using var dataStream = dataClient.GetStream();
            await data.CopyToAsync(dataStream, ct);
        }

        await ReadExpectedStatusAsync(226, ct);
    }

    private async ValueTask DeleteItemCore(string path, CancellationToken ct)
    {
        var item = await GetItemAsync(path, ct);
        await DeleteItemAsync(item, ct);
    }

    private async ValueTask DeleteItemAsync(FtpItem item, CancellationToken ct)
    {
        if (item.Type == FtpItemType.File)
        {
            await ExecuteAsync($"DELE {item.Path}", 250, ct);
        }
        else
        {
            var items = await GetListItemCore(item.Path, false, ct);
            var array = items.ToArray();

            foreach (var i in array)
            {
                await DeleteItemAsync(i, ct);
            }

            await ExecuteAsync($"RMD {item.Path}", 250, ct);
        }
    }

    private async ValueTask<Memory<FtpItem>> GetListItemCore(
        string path,
        bool isSingle,
        CancellationToken ct
    )
    {
        var parameters = await GetPasvAsync(ct);
        var fileEntries = new List<FtpItem>();

        using (var dataClient = new TcpClient(parameters.Host, parameters.Port))
        {
            await ExecuteAsync($"LIST {path}", 150, ct);
            await using var dataStream = dataClient.GetStream();
            using var dataReader = new StreamReader(dataStream);

            while (await dataReader.ReadLineAsync(ct) is { } entry)
            {
                if (isSingle)
                {
                    fileEntries.Add(
                        new(path, entry.StartsWith('d') ? FtpItemType.Directory : FtpItemType.File)
                    );
                }
                else
                {
                    fileEntries.Add(ParseFtpItem(entry, path));
                }
            }
        }

        await ReadExpectedStatusAsync(226, ct);

        return fileEntries.ToArray();
    }

    private async ValueTask<(string Host, int Port)> GetPasvAsync(CancellationToken ct)
    {
        var response = await ExecuteAsync("PASV", 227, ct);
        var valuesStartIndex = response.Span.IndexOf('(');
        var valuesEndIndex = response.Span.LastIndexOf(')');
        var valuesStr = response.Slice(valuesStartIndex + 1, valuesEndIndex - valuesStartIndex - 1);
        var values = valuesStr.Span.Split(',');
        var parameters = ParsePasvResponse(values);

        return parameters;
    }

    private FtpItem ParseFtpItem(ReadOnlySpan<char> entry, string path)
    {
        var type = entry.StartsWith('d') ? FtpItemType.Directory : FtpItemType.File;
        var twoDotsIndex = entry.IndexOf(':');

        if (twoDotsIndex != -1)
        {
            var slice = entry.Slice(twoDotsIndex);
            var name = slice.Slice(slice.IndexOf(' ') + 1);
            var entryPath = Path.Combine(path, name.ToString()).Replace('\\', '/');

            return new(entryPath, type);
        }

        var segments = entry.Split(' ');
        var findYear = false;
        var findMonth = false;

        foreach (var segment in segments)
        {
            if (findYear)
            {
                return new(Path.Combine(path, entry[segment].ToString()).Replace('\\', '/'), type);
            }

            if (Months.Span.Contains(entry[segment].ToString()))
            {
                findMonth = true;
            }

            if (segment.End.Value - segment.Start.Value == 4 && findMonth)
            {
                findYear = true;
            }
        }

        throw new InvalidOperationException($"Invalid FTP entry {entry}");
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
        var response = await ExecuteAsync("PWD", 257, ct);
        var firstQuote = response.Span.IndexOf('\"');
        var lastQuote = response.Span.LastIndexOf('\"');
        var path = response.Slice(firstQuote + 1, lastQuote - firstQuote - 1);

        return new(path.Span.ToString(), FtpItemType.Directory);
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
        await ReadExpectedStatusAsync(reader, 220, ct);
        await writer.WriteLineAsync($"USER {username}");
        await ReadExpectedStatusAsync(reader, 331, ct);
        await writer.WriteLineAsync($"PASS {password}");
        await ReadExpectedStatusAsync(reader, 230, ct);

        return new(controlClient, stream, writer, reader);
    }

    private async ValueTask<ReadOnlyMemory<char>> ExecuteAsync(
        string command,
        int statusCode,
        CancellationToken ct
    )
    {
        await _streamWriter.WriteLineAsync(command);

        return await ReadExpectedStatusAsync(statusCode, ct);
    }

    private ValueTask<ReadOnlyMemory<char>> ReadExpectedStatusAsync(
        int statusCode,
        CancellationToken ct
    )
    {
        return ReadExpectedStatusAsync(_streamReader, statusCode, ct);
    }

    private static async ValueTask<ReadOnlyMemory<char>> ReadExpectedStatusAsync(
        StreamReader reader,
        int statusCode,
        CancellationToken ct
    )
    {
        var str = await reader.ReadLineAsync(ct);
        var strSpan = str.ThrowIfNull();

        if (strSpan.StartsWith(statusCode.ToString()))
        {
            return strSpan.ToArray();
        }

        throw new ExpectedFtpStatusCodeException(strSpan, statusCode);
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
