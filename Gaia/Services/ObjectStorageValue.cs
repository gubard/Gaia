namespace Gaia.Services;

public interface IObjectStorageValue
{
    public static abstract string GetObjectStorageKey();
    public static abstract string GetObjectStorageKey(Guid id);
}

public abstract record ObjectStorageValue<TSelf> : IObjectStorageValue
{
    public static string GetObjectStorageKey()
    {
        return $"{typeof(TSelf).FullName}";
    }

    public static string GetObjectStorageKey(Guid id)
    {
        return $"{typeof(TSelf).FullName}#{id}";
    }
}
