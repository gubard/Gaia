namespace Gaia.Helpers;

public static class DirectoryInfoExtension
{
    extension(DirectoryInfo directoryInfo)
    {
        public FileInfo ToFile(string fileName)
        {
            return new(Path.Combine(directoryInfo.FullName, fileName));
        }

        public DirectoryInfo Combine(string segment)
        {
            return new(Path.Combine(directoryInfo.FullName, segment));
        }
    }
}
