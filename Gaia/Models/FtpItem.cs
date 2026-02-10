namespace Gaia.Models;

public sealed class FtpItem
{
    public FtpItem(string path, FtpItemType type)
    {
        Path = path;
        Type = type;
    }

    public string Path { get; }
    public FtpItemType Type { get; }
}

public enum FtpItemType
{
    File,
    Directory,
}
