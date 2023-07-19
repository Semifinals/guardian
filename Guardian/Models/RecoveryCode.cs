namespace Semifinals.Guardian.Models;

/// <summary>
/// A code used to recover a first-party account.
/// </summary>
public class RecoveryCode
{
    /// <summary>
    /// The associated identity that the code is for.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; }
    
    /// <summary>
    /// The unique recovery code.
    /// </summary>
    [JsonPropertyName("code")]
    public string Code { get; }

    /// <summary>
    /// The type requests the recovery code can be used for.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; }
    

    public RecoveryCode(string id, string code, string type)
    {
        Id = id;
        Code = code;
        Type = type;
    }
}
