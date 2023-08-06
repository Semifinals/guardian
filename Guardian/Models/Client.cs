namespace Semifinals.Guardian.Models;

/// <summary>
/// An oauth2 client.
/// </summary>
public class Client
{
    /// <summary>
    /// The client ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; }

    /// <summary>
    /// The client secret.
    /// </summary>
    [JsonPropertyName("secret")]
    public string Secret { get; }

    public Client(
        string clientId,
        string clientSecret)
    {
        Id = clientId;
        Secret = clientSecret;
    }
}
