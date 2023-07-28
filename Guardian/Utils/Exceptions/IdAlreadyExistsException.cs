namespace Semifinals.Guardian.Utils.Exceptions;

public class IdAlreadyExistsException : AlreadyExistsException
{
    public string Id;

    public IdAlreadyExistsException(string id) : base()
    {
        Id = id;
    }
}
