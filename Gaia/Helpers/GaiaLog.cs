using Microsoft.Extensions.Logging;

namespace Gaia.Helpers;

public static partial class GaiaLog
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "Try {TryCount} exception")]
    public static partial void TryException(this ILogger logger, int tryCount, Exception exception);
}
