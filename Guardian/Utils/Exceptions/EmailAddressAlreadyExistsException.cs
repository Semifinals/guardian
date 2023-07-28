namespace Semifinals.Guardian.Utils.Exceptions;

public class EmailAddressAlreadyExistsException : AlreadyExistsException
{
    public string EmailAddress;
    
    public EmailAddressAlreadyExistsException(string emailAddress) : base()
    {
        EmailAddress = emailAddress;
    }
}
