namespace Semifinals.Guardian.Utils.Exceptions;

public class NotFoundException : Exception
{
    public string Id;
    
    public NotFoundException(string id) : base()
    {
        Id = id;
    }
}
