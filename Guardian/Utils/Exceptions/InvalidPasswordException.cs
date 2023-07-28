namespace Semifinals.Guardian.Utils.Exceptions;

public class InvalidPasswordException : Exception
{
    public string Password;

    public InvalidPasswordException(string password) : base()
    {
        Password = password;
    }
}
