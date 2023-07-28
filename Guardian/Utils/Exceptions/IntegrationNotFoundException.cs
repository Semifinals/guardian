namespace Semifinals.Guardian.Utils.Exceptions;

public class IntegrationNotFoundException : NotFoundException
{
    public IntegrationNotFoundException(string id) : base(id) { }
}
