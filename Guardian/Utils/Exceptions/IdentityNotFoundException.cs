namespace Semifinals.Guardian.Utils.Exceptions;

public class IdentityNotFoundException : NotFoundException
{
    public IdentityNotFoundException(string id) : base(id) { }
}
