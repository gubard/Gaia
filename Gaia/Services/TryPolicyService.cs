using System.Runtime.CompilerServices;
using Gaia.Helpers;
using Gaia.Models;
using Microsoft.Extensions.Logging;

namespace Gaia.Services;

public interface ITryPolicyService
{
    T Try<T>(Func<T> func)
        where T : IValidationErrors, new();

    ConfiguredValueTaskAwaitable<T> TryAsync<T>(Func<ConfiguredValueTaskAwaitable<T>> func)
        where T : IValidationErrors, new();
}

public sealed class TryPolicyService : ITryPolicyService
{
    public TryPolicyService(
        byte tryCount,
        TimeSpan delay,
        Action<Exception> onError,
        ILogger logger,
        Action<ExceptionsValidationError> onCritical
    )
    {
        _tryCount = tryCount;
        _delay = delay;
        _onError = onError;
        _logger = logger;
        _onCritical = onCritical;
    }

    public T Try<T>(Func<T> func)
        where T : IValidationErrors, new()
    {
        var count = 0;
        var exceptions = new Exception[_tryCount];

        while (count < _tryCount)
        {
            try
            {
                var value = func();

                return value;
            }
            catch (Exception exception)
            {
                _logger.TryException(count, exception);
                exceptions[count] = exception;
                _onError.Invoke(exception);
                Thread.Sleep(_delay);
            }

            count++;
        }

        var error = new ExceptionsValidationError(exceptions);
        _onCritical.Invoke(error);
        var result = new T();
        result.ValidationErrors.Add(error);

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
    private readonly Action<ExceptionsValidationError> _onCritical;
    private readonly ILogger _logger;

    private async ValueTask<T> TryCore<T>(Func<ConfiguredValueTaskAwaitable<T>> func)
        where T : IValidationErrors, new()
    {
        var count = 0;
        var exceptions = new Exception[_tryCount];

        while (count < _tryCount)
        {
            try
            {
                var value = await func();

                return value;
            }
            catch (Exception exception)
            {
                _logger.TryException(count, exception);
                exceptions[count] = exception;
                _onError.Invoke(exception);
                await Task.Delay(_delay);
            }

            count++;
        }

        var error = new ExceptionsValidationError(exceptions);
        _onCritical.Invoke(error);
        var result = new T();
        result.ValidationErrors.Add(error);

        return result;
    }
}
