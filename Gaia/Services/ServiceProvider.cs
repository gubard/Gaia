namespace Gaia.Services;

public interface IServiceProvider
{
    object GetService(Type type);

    T GetService<T>()
        where T : notnull;
}
