﻿namespace Semifinals.Guardian.Models;

/// <summary>
/// A third-party account integration used to login.
/// </summary>
public class Integration
{
    /// <summary>
    /// A composite key containing the user's unique identifier from the 
    /// platform, as well as the name of the platform itself.
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

    /// <summary>
    /// The user's unique identifier from the platform.
    /// </summary>
    public string UserId { get; }

    public Integration(
        string identityId,
        string platform,
        string userId)
    {
        Id = GetCompositeId(platform, userId);
        IdentityId = identityId;
        Platform = platform;
        UserId = userId;
    }

    public static string GetCompositeId(string platform, string userId)
    {
        return platform + userId;
    }
}
