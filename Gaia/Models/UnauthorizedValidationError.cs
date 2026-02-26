namespace Gaia.Models;

public sealed class UnauthorizedValidationError : ValidationError
{
    public override string ToString()
    {
        return "App unauthorized";
    }
}
