namespace Semifinals.Guardian.Utils.Exceptions;

public class InvalidRecoveryCodeException : Exception
{
    public string Code;

    public InvalidRecoveryCodeException(string code) : base()
    {
        Code = code;
    }
}
