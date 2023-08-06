namespace Semifinals.Guardian.Utils.Exceptions;

public class TokenNotFoundException : NotFoundException
{
    public int IssuedAt;
    
    public TokenNotFoundException(string id, int iat) : base(id)
    {
        IssuedAt = iat;
    }
}
