using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Gaia.Helpers;

public static class FileInfoExtension
{
    extension(FileInfo file)
    {
        public void WriteAllText(string text)
        {
            File.WriteAllText(file.FullName, text);
        }

        public string ReadAllText()
        {
            return File.ReadAllText(file.FullName);
        }

        public Task<string[]> ReadAllLinesAsync(CancellationToken ct)
        {
            return File.ReadAllLinesAsync(file.FullName, ct);
        }

        public string[] ReadAllLines()
        {
            return File.ReadAllLines(file.FullName);
        }

        public Task<string> ReadAllTextAsync(CancellationToken ct)
        {
            return File.ReadAllTextAsync(file.FullName, ct);
        }

        public Task WriteAllTextAsync(string text, CancellationToken ct)
        {
            return File.WriteAllTextAsync(file.FullName, text, ct);
        }

        public FileInfo FileInSameDir(string fileName)
        {
            return file.Directory.ThrowIfNull().ToFile(fileName);
        }

        public string GetFileNameWithoutExtension()
        {
            return Path.GetFileNameWithoutExtension(file.FullName);
        }

        public Stream OpenWriteOrCreate()
        {
            return file.Exists ? file.OpenWrite() : file.Create();
        }

        public ConfiguredValueTaskAwaitable<T?> DeserializeJsonAsync<T>(
            JsonSerializerOptions options,
            CancellationToken ct
        )
        {
            return file.DeserializeJsonCore<T>(options, ct).ConfigureAwait(false);
        }

        private async ValueTask<T?> DeserializeJsonCore<T>(
            JsonSerializerOptions options,
            CancellationToken ct
        )
        {
            if (!file.Exists)
            {
                return default;
            }

            await using var stream = file.OpenRead();

            return await JsonSerializer.DeserializeAsync<T>(stream, options, ct);
        }

        public ConfiguredValueTaskAwaitable SerializeJsonAsync<T>(
            T value,
            JsonSerializerOptions options,
            CancellationToken ct
        )
        {
            return file.SerializeJsonCore(value, options, ct).ConfigureAwait(false);
        }

        private async ValueTask SerializeJsonCore<T>(
            T value,
            JsonSerializerOptions options,
            CancellationToken ct
        )
        {
            await using var stream = file.OpenWriteOrCreate();
            await JsonSerializer.SerializeAsync(stream, value, options, ct);
        }

        public T? DeserializeJson<T>(JsonSerializerOptions options)
        {
            if (!file.Exists)
            {
                return default;
            }

            using var stream = file.OpenRead();

            return JsonSerializer.Deserialize<T>(stream, options);
        }

        public void SerializeJson<T>(T value, JsonSerializerOptions options)
        {
            using var stream = file.OpenWriteOrCreate();
            JsonSerializer.Serialize(stream, value, options);
        }
    }
}
