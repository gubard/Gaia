using System.Runtime.CompilerServices;
using Gaia.Helpers;
using Gaia.Models;
using Microsoft.Extensions.Logging;

namespace Gaia.Services;

public interface ITryPolicyService
{
    Result Try(Func<Result> func);
    ConfiguredValueTaskAwaitable<Result> TryAsync(Func<ConfiguredValueTaskAwaitable<Result>> func);
    Result<T> Try<T>(Func<Result<T>> func);

    ConfiguredValueTaskAwaitable<Result<T>> TryAsync<T>(
        Func<ConfiguredValueTaskAwaitable<Result<T>>> func
    );
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

    public Result Try(Func<Result> func)
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
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                _logger.TryException(count + 1, exception);
                exceptions[count] = exception;
                _onError.Invoke(exception);
                Thread.Sleep(_delay);
            }

            count++;
        }

        var error = new ExceptionsValidationError(exceptions);
        _onCritical.Invoke(error);

        return error;
    }

    public Result<T> Try<T>(Func<Result<T>> func)
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
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                _logger.TryException(count + 1, exception);
                exceptions[count] = exception;
                _onError.Invoke(exception);
                Thread.Sleep(_delay);
            }

            count++;
        }

        var error = new ExceptionsValidationError(exceptions);
        _onCritical.Invoke(error);

        return error;
    }

    public ConfiguredValueTaskAwaitable<Result> TryAsync(
        Func<ConfiguredValueTaskAwaitable<Result>> func
    )
    {
        return TryCore(func).ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable<Result<T>> TryAsync<T>(
        Func<ConfiguredValueTaskAwaitable<Result<T>>> func
    )
    {
        return TryCore(func).ConfigureAwait(false);
    }

    private readonly byte _tryCount;
    private readonly TimeSpan _delay;
    private readonly Action<Exception> _onError;
    private readonly Action<ExceptionsValidationError> _onCritical;
    private readonly ILogger _logger;

    private async ValueTask<Result> TryCore(Func<ConfiguredValueTaskAwaitable<Result>> func)
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
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                _logger.TryException(count + 1, exception);
                exceptions[count] = exception;
                _onError.Invoke(exception);
                await Task.Delay(_delay);
            }

            count++;
        }

        var error = new ExceptionsValidationError(exceptions);
        _onCritical.Invoke(error);

        return error;
    }

    private async ValueTask<Result<T>> TryCore<T>(
        Func<ConfiguredValueTaskAwaitable<Result<T>>> func
    )
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
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                _logger.TryException(count + 1, exception);
                exceptions[count] = exception;
                _onError.Invoke(exception);
                await Task.Delay(_delay);
            }

            count++;
        }

        var error = new ExceptionsValidationError(exceptions);
        _onCritical.Invoke(error);

        return error;
    }
}
