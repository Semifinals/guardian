namespace Semifinals.Guardian.Utils.Exceptions;

public class AccountNotFoundException : NotFoundException
{
    public AccountNotFoundException(string id) : base(id) { }
}
