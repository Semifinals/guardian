namespace Semifinals.Guardian.Models;

/// <summary>
/// The content of a resposne containing tokens for authentication.
/// </summary>
public class TokenResponse
{
    /// <summary>
    /// The access token used to make API requests.
    /// </summary>
    [JsonPropertyName("access_token")]
    public string AccessToken { get; }

    /// <summary>
    /// The refresh token to regenerate the access token if included.
    /// </summary>
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; }

    /// <summary>
    /// The number of seconds until the access token expires.
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; }

    /// <summary>
    /// The type of token used.
    /// </summary>
    [JsonPropertyName("token_type")]
    public string TokenType { get; }

    public TokenResponse(
        string accessToken,
        string? refreshToken,
        int expiresIn,
        string tokenType)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        ExpiresIn = expiresIn;
        TokenType = tokenType;
    }
}