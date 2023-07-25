using crypto = BCrypt.Net.BCrypt;

namespace Semifinals.Guardian.Utils;

public class Crypto
{
    /// <summary>
    /// Hash a value.
    /// </summary>
    /// <param name="value">The value to hash</param>
    /// <returns>The resulting hash</returns>
    public static string Hash(string value)
    {
        return crypto.HashPassword(value);
    }

    /// <summary>
    /// Verify that a hash is the result of a value.
    /// </summary>
    /// <param name="value">The value to test</param>
    /// <param name="hash">The expected hash</param>
    /// <returns>Whether or not the value is valid</returns>
    public static bool Verify(string value, string hash)
    {
        return crypto.Verify(value, hash);
    }
}
