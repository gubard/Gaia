namespace Gaia.Models;

public class DayOfYear : IComparable<DayOfYear>
{
    public byte Day { get; set; }
    public Month Month { get; set; }

    public int CompareTo(DayOfYear? other)
    {
        if (other == null)
        {
            return 1;
        }

        var year = DateTime.Now.Year;
        var x = new DateOnly(
            year,
            (int)Month,
            Math.Min(DateTime.DaysInMonth(year, (int)Month), Day)
        );
        var y = new DateOnly(
            year,
            (int)other.Month,
            Math.Min(DateTime.DaysInMonth(year, (int)other.Month), other.Day)
        );

        return x.CompareTo(y);
    }
}
