namespace Gaia.Services;

public interface IServiceProvider
{
    T GetService<T>() where T : notnull;
    object GetService(Type type);
}