namespace Semifinals.Guardian.Models;

/// <summary>
/// A code used to recover a first-party account.
/// </summary>
public class RecoveryCode
{
    /// <summary>
    /// A composite ID to identify the recovery code.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; }
    
    /// <summary>
    /// The unique recovery code.
    /// </summary>
    [JsonPropertyName("code")]
    public string Code { get; }

    /// <summary>
    /// The associated identity that the code is for.
    /// </summary>
    public string IdentityId { get; }

    /// <summary>
    /// The type requests the recovery code can be used for.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; }
    

    public RecoveryCode(string identityId, string code, string type)
    {
        Id = GetCompositeId(identityId, type);
        Code = code;
        IdentityId = identityId;
        Type = type;
    }

    /// <summary>
    /// Generate a composite ID for a recovery code.
    /// </summary>
    /// <param name="identityId">The user's identity ID</param>
    /// <param name="type">The type of recovery code this represents</param>
    /// <returns>The composite ID of the two properties</returns>
    public static string GetCompositeId(string identityId, string type)
    {
        return $"{type}:{identityId}";
    }
}
