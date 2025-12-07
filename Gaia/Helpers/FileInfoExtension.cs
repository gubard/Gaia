namespace Gaia.Helpers;

public static class FileInfoExtension
{
    extension(FileInfo fileInfo)
    {
        public void WriteAllText(string text)
        {
            File.WriteAllText(fileInfo.FullName, text);
        }

        public string ReadAllText()
        {
            return File.ReadAllText(fileInfo.FullName);
        }

        public Task<string> ReadAllTextAsync(CancellationToken ct)
        {
            return File.ReadAllTextAsync(fileInfo.FullName, ct);
        }

        public Task WriteAllTextAsync(string text, CancellationToken ct)
        {
            return File.WriteAllTextAsync(fileInfo.FullName, text, ct);
        }

        public FileInfo FileInSameDir(string fileName)
        {
            return fileInfo.Directory.ThrowIfNull().ToFile(fileName);
        }

        public string GetFileNameWithoutExtension()
        {
            return Path.GetFileNameWithoutExtension(fileInfo.FullName);
        }
    }
}