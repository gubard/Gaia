namespace Gaia.Services;

public interface IServiceProvider
{
    T GetService<T>() where T : notnull;
}