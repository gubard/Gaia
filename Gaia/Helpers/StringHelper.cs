using System.Text.RegularExpressions;

namespace Gaia.Helpers;

public static partial class StringHelper
{
    [GeneratedRegex(
        @"^(?:[a-zA-Z0-9_'^&/+-])+(?:\.(?:[a-zA-Z0-9_'^&/+-])+)*@(?:[a-zA-Z0-9-]+\.)+[a-zA-Z]{2,}$",
        RegexOptions.Compiled
    )]
    private static partial Regex CreateEmailRegex();

    public const string UpperLatin = "QAZWSXEDCRFVTGBYHNUJMIKOP";
    public const string LowerLatin = "qazwsxedcrfvtgbyhnujmikolp";
    public const string Number = "0123456789";
    public const string SpecialSymbols = "~`!@#$%^&*()_+\\|}{[]'\";:/?.>,<";
    public static readonly Regex EmailRegex;

    static StringHelper()
    {
        EmailRegex = CreateEmailRegex();
    }
}
