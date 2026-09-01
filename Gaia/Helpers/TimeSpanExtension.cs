namespace Gaia.Helpers;

public static class TimeSpanExtension
{
    public static TimeOnly ToTimeOnly(this TimeSpan timeSpan)
    {
        return new TimeOnly(timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
    }
}
