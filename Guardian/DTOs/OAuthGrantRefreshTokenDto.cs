namespace Semifinals.Guardian.DTOs;

public class OAuthGrantRefreshTokenDto
{
    [JsonPropertyName("grant_type")]
    public string GrantType { get; set; } = null!;

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; } = null!;

    [JsonPropertyName("scope")]
    public string? Scope { get; set; }
}

public class OAuthGrantRefreshTokenDtoValidator
    : AbstractValidator<OAuthGrantRefreshTokenDto>
{
    public OAuthGrantRefreshTokenDtoValidator()
    {
        RuleFor(x => x.GrantType)
            .Equal("refresh_token");

        RuleFor(x => x.RefreshToken)
            .NotEmpty();

        RuleFor(x => x.Scope)
            .NotEmpty()
            .When(x => !string.IsNullOrEmpty(x.Scope));
    }
}
