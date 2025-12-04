namespace Gaia.Helpers;

public static class DirectoryInfoExtension
{
    public static FileInfo ToFile(this DirectoryInfo directoryInfo, string fileName)
    {
        return new(Path.Combine(directoryInfo.FullName, fileName));
    }
}