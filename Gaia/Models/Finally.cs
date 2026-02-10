namespace Gaia.Models;

public sealed class Finally : IDisposable
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

public sealed class FinallyAsync : IAsyncDisposable
{
    private readonly Func<ValueTask> _dispose;

    public FinallyAsync(Func<ValueTask> dispose)
    {
        _dispose = dispose;
    }

    public ValueTask DisposeAsync()
    {
        return _dispose.Invoke();
    }
}
