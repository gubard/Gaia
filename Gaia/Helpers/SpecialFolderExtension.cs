namespace Gaia.Helpers;

public static class SpecialFolderExtension
{
    extension(Environment.SpecialFolder folder)
    {
        public string GetPath()
        {
            return Environment.GetFolderPath(folder);
        }

        public DirectoryInfo GetDir()
        {
            return new(folder.GetPath());
        }
    }
}
