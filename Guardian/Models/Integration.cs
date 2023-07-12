namespace Semifinals.Guardian.Models;

/// <summary>
/// A third-party account integration used to login.
/// </summary>
public class Integration
{
    /// <summary>
    /// The user's unique identifier from the other platform.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; }

    /// <summary>
    /// The associated identity.
    /// </summary>
    [JsonPropertyName("identityId")]
    public string IdentityId { get; }

    /// <summary>
    /// The platform the integration originates from.
    /// </summary>
    [JsonPropertyName("platform")]
    public string Platform { get; }

    public Integration(
        string id,
        string identityId,
        string platform)
    {
        Id = id;
        IdentityId = identityId;
        Platform = platform;
    }
}
