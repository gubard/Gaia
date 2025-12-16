namespace Gaia.Models;

public class Dis : IDisposable
{
    private readonly Action _dispose;

    public Dis(Action dispose)
    {
        _dispose = dispose;
    }

    public void Dispose()
    {
        _dispose.Invoke();
    }
}
