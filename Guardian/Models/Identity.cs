namespace Semifinals.Guardian.Models;

/// <summary>
/// The identity of a user, agnostic to login method.
/// </summary>
public class Identity
{
    /// <summary>
    /// The identity's unique identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; }

    /// <summary>
    /// A dictionary containing all integrations associated with the account,
    /// where the key is the platform and the value is the user's unique
    /// identifier on that platform.
    /// </summary>
    [JsonPropertyName("integrations")]
    public IDictionary<string, string> Integrations { get; }

    public Identity(
        string id,
        IDictionary<string, string>? integrations = null)
    {
        Id = id;
        Integrations = integrations ?? new Dictionary<string, string>();
    }
}
