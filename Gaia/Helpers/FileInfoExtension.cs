namespace Gaia.Helpers;

public static class FileInfoExtension
{
    extension(FileInfo fileInfo)
    {
        public Task<string> ReadAllTextAsync(CancellationToken ct)
        {
            return File.ReadAllTextAsync(fileInfo.FullName, ct);
        }

        public Task WriteAllTextAsync(string text, CancellationToken ct)
        {
            return File.WriteAllTextAsync(fileInfo.FullName, text, ct);
        }
    }

}