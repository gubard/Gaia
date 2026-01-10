using System.Runtime.CompilerServices;
using Gaia.Models;

namespace Gaia.Services;

public interface ITryPolicyService
{
    T Try<T>(Func<T> func)
        where T : IValidationErrors, new();

    ConfiguredValueTaskAwaitable<T> TryAsync<T>(Func<ConfiguredValueTaskAwaitable<T>> func)
        where T : IValidationErrors, new();
}

public class TryPolicyService : ITryPolicyService
{
    public TryPolicyService(byte tryCount, TimeSpan delay, Action<Exception> onError)
    {
        _tryCount = tryCount;
        _delay = delay;
        _onError = onError;
    }

    public T Try<T>(Func<T> func)
        where T : IValidationErrors, new()
    {
        var count = 0;
        var exceptions = new List<Exception>();

        while (count++ < _tryCount)
        {
            try
            {
                var value = func();

                return value;
            }
            catch (Exception exception)
            {
                exceptions.Add(exception);
                _onError?.Invoke(exception);
                Thread.Sleep(_delay);
            }
        }

        var result = new T();
        var exceptionsArray = exceptions.ToArray();
        result.ValidationErrors.Add(new ExceptionsValidationError(exceptionsArray));

        return result;
    }

    public ConfiguredValueTaskAwaitable<T> TryAsync<T>(Func<ConfiguredValueTaskAwaitable<T>> func)
        where T : IValidationErrors, new()
    {
        return TryCore(func).ConfigureAwait(false);
    }

    private readonly byte _tryCount;
    private readonly TimeSpan _delay;
    private readonly Action<Exception> _onError;

    private async ValueTask<T> TryCore<T>(Func<ConfiguredValueTaskAwaitable<T>> func)
        where T : IValidationErrors, new()
    {
        var count = 0;
        var exceptions = new List<Exception>();

        while (count++ < _tryCount)
        {
            try
            {
                var value = await func();

                return value;
            }
            catch (Exception exception)
            {
                exceptions.Add(exception);
                _onError?.Invoke(exception);
                await Task.Delay(_delay);
            }
        }

        var result = new T();
        var exceptionsArray = exceptions.ToArray();
        result.ValidationErrors.Add(new ExceptionsValidationError(exceptionsArray));

        return result;
    }
}
