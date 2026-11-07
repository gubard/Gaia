namespace Gaia.Helpers;

public static class DateTimeExtension
{
    public static DateOnly? ToDateOnly(this DateTime? dateTime)
    {
        if (dateTime.HasValue)
        {
            return dateTime.Value.ToDateOnly();
        }

        return null;
    }

    public static DateTime? ToAddDaysOrNull(this DateTime? dateTime, double days)
    {
        if (dateTime.HasValue)
        {
            return dateTime.Value.AddDays(days);
        }

        return null;
    }

    public static DateOnly ToDateOnly(this DateTime dateTime)
    {
        return DateOnly.FromDateTime(dateTime);
    }

    public static string MonthToUaString(this DateTime dateTime)
    {
        return dateTime.ToString("MMMM", CultureHelper.Ukranian);
    }
}
