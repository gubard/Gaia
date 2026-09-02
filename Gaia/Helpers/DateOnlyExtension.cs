namespace Gaia.Helpers;

public static class DateOnlyExtension
{
    public static DateOnly WithYear(this DateOnly date, int year)
    {
        var day = Math.Min(DateTime.DaysInMonth(year, date.Month), date.Day);

        return new(year, date.Month, day);
    }

    public static DateOnly WithDay(this DateOnly date, int day)
    {
        return new(date.Year, date.Month, day);
    }

    public static DateOnly WithMonth(this DateOnly date, int month)
    {
        return new(date.Year, month, date.Day);
    }

    public static DateTime ToDateTime(this DateOnly date, DateTimeKind kind)
    {
        return date.ToDateTime(TimeOnly.MinValue, kind);
    }

    public static DateTime ToDateTime(this DateOnly date)
    {
        return date.ToDateTime(TimeOnly.MinValue);
    }

    public static DateTimeOffset ToDateTimeOffset(this DateOnly date)
    {
        return new(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeOffset.Now.Offset);
    }
}
