using Gaia.Models;

namespace Gaia.Services;

public interface ITryPolicyService
{
    event Action<Exception> OnError;
    event Action OnSuccess;
    T Try<T>(Func<T> func) where T : IValidationErrors, new();
    ValueTask<T> TryAsync<T>(Func<ValueTask<T>> func) where T : IValidationErrors, new();
}

public class TryPolicyService : ITryPolicyService
{
    private readonly byte _tryCount;
    private readonly TimeSpan _delay;

    public TryPolicyService(byte tryCount, TimeSpan delay)
    {
        _tryCount = tryCount;
        _delay = delay;
    }

    public event Action<Exception>? OnError;
    public event Action? OnSuccess;


    public T Try<T>(Func<T> func) where T : IValidationErrors, new()
    {
        var count = 0;
        var exceptions = new List<Exception>();

        while (count++ < _tryCount)
        {
            try
            {
                var value = func();
                OnSuccess?.Invoke();

                return value;
            }
            catch (Exception exception)
            {
                exceptions.Add(exception);
                OnError?.Invoke(exception);
                Thread.Sleep(_delay);
            }
        }

        var result = new T();
        var exceptionsArray = exceptions.ToArray();
        result.ValidationErrors.Add(new ExceptionsValidationError(exceptionsArray));

        return result;
    }

    public async ValueTask<T> TryAsync<T>(Func<ValueTask<T>> func) where T : IValidationErrors, new()
    {
        var count = 0;
        var exceptions = new List<Exception>();

        while (count++ < _tryCount)
        {
            try
            {
                var value = await func();
                OnSuccess?.Invoke();

                return value;
            }
            catch (Exception exception)
            {
                exceptions.Add(exception);
                OnError?.Invoke(exception);
                await Task.Delay(_delay);
            }
        }

        var result = new T();
        var exceptionsArray = exceptions.ToArray();
        result.ValidationErrors.Add(new ExceptionsValidationError(exceptionsArray));

        return result;
    }
}