namespace Semifinals.Guardian.Utils.Exceptions;

public class ClientNotFoundException : NotFoundException
{
    public ClientNotFoundException(string id) : base(id) { }
}
