namespace Gaia.Models;

public class Finally : IDisposable
{
    private readonly Action _dispose;

    public Finally(Action dispose)
    {
        _dispose = dispose;
    }

    public void Dispose()
    {
        _dispose.Invoke();
    }
}
