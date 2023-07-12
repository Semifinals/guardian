namespace Semifinals.Guardian.Models;

/// <summary>
/// A first-party Semifinals account used to login.
/// </summary>
public class Account
{
    /// <summary>
    /// The ID of the identity the account is associated with.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; }

    /// <summary>
    /// The email address associated with the account.
    /// </summary>
    [JsonPropertyName("emailAddress")]
    public string EmailAddress { get; }

    /// <summary>
    /// The hashed password.
    /// </summary>
    [JsonPropertyName("passwordHashed")]
    public string PasswordHashed { get; }

    /// <summary>
    /// The salt used to hash the password.
    /// </summary>
    [JsonPropertyName("salt")]
    public string Salt { get; }

    /// <summary>
    /// Whether or not the user has verified their email address.
    /// </summary>
    [JsonPropertyName("verified")]
    public bool Verified { get; }

    public Account(
        string id,
        string emailAddress,
        string passwordHashed,
        string salt,
        bool verified)
    {
        Id = id;
        EmailAddress = emailAddress;
        PasswordHashed = passwordHashed;
        Salt = salt;
        Verified = verified;
    }
}
