namespace Gaia.Errors;

public abstract class IdentityValidationError : ValidationError
{
    protected IdentityValidationError(string identity)
    {
        Identity = identity;
    }

    public string Identity { get; }
}

public sealed class DuplicationValidationError : IdentityValidationError
{
    public DuplicationValidationError(string identity) : base(identity)
    {
    }
}

public sealed class NotFoundValidationError : IdentityValidationError
{
    public NotFoundValidationError(string identity) : base(identity)
    {
    }
}

public sealed class AlreadyExistsValidationError : IdentityValidationError
{
    public AlreadyExistsValidationError(string identity) : base(identity)
    {
    }
}