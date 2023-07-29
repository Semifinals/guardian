namespace Semifinals.Guardian.DTOs;

public class OAuthGrantRefreshTokenDto
{
    [JsonPropertyName("grant_type")]
    public string GrantType;

    [JsonPropertyName("refresh_token")]
    public string RefreshToken;

    [JsonPropertyName("scope")]
    public string? Scope;

    public OAuthGrantRefreshTokenDto(
        string grantType,
        string refreshToken,
        string? scope)
    {
        GrantType = grantType;
        RefreshToken = refreshToken;
        Scope = scope;
    }
}

public class OAuthGrantRefreshTokenDtoValidator
    : AbstractValidator<OAuthGrantRefreshTokenDto>
{
    public OAuthGrantRefreshTokenDtoValidator()
    {
        RuleFor(x => x.GrantType)
            .Equal("client_credentials");

        RuleFor(x => x.RefreshToken)
            .NotEmpty();

        RuleFor(x => x.Scope)
            .NotEmpty()
            .When(x => !string.IsNullOrEmpty(x.Scope));
    }
}
