namespace Semifinals.Guardian.Models;

/// <summary>
/// A code used to recover a first-party account.
/// </summary>
public class Token
{
    /// <summary>
    /// A composite ID to identify the refresh token.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; }

    /// <summary>
    /// The refresh token.
    /// </summary>
    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; }

    /// <summary>
    /// The associated identity that the code is for.
    /// </summary>
    [JsonPropertyName("identityId")]
    public string IdentityId { get; }

    public Token(string identityId, string token, int iat)
    {
        Id = GetCompositeId(identityId, iat);
        RefreshToken = token;
        IdentityId = identityId;
    }
    
    /// <summary>
    /// Generate a composite ID for a recovery code.
    /// </summary>
    /// <param name="identityId">The user's identity ID</param>
    /// <param name="iat">The unix timestamp for when the token was generated</param>
    /// <returns>The composite ID of the two properties</returns>
    public static string GetCompositeId(string identityId, int iat)
    {
        return $"{identityId}:{iat}";
    }
}
