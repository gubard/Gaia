namespace Gaia.Helpers;

public static class FileInfoExtension
{
    extension(FileInfo fileInfo)
    {
        public Task<string> ReadAllTextAsync()
        {
            return File.ReadAllTextAsync(fileInfo.FullName);
        }

        public Task WriteAllTextAsync(string text)
        {
            return File.WriteAllTextAsync(fileInfo.FullName, text);
        }
    }

}