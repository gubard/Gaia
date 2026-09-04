namespace Gaia.Helpers;

public static class EnumHelper
{
    public static Enum[] GetValues(Type type)
    {
        if (Values.TryGetValue(type, out var values))
        {
            return values;
        }

        var enumValues = Enum.GetValuesAsUnderlyingType(type);
        values = new Enum[enumValues.Length];

        for (var i = 0; i < enumValues.Length; i++)
        {
            values[i] = Enum.Parse(
                    type,
                    enumValues.GetValue(i).ThrowIfNull().ToString().ThrowIfNull()
                )
                .Cast<Enum>();
        }

        Values[type] = values;

        return values;
    }

    private static readonly Dictionary<Type, Enum[]> Values = new();
}

public static class EnumHelper<T>
    where T : Enum
{
    public static readonly IEnumerable<T> Values = Enum.GetValuesAsUnderlyingType(typeof(T))
        .Cast<T>()
        .ToArray();
}
