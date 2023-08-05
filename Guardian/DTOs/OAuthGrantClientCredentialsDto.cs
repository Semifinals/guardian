namespace Semifinals.Guardian.DTOs;

public class OAuthGrantClientCredentialsDto
{
    [JsonPropertyName("grant_type")]
    public string GrantType { get; set; } = null!;

    [JsonPropertyName("scope")]
    public string? Scope { get; set; }
}

public class OAuthGrantClientCredentialsDtoValidator
    : AbstractValidator<OAuthGrantClientCredentialsDto>
{
    public OAuthGrantClientCredentialsDtoValidator()
    {
        RuleFor(x => x.GrantType)
            .Equal("client_credentials");

        RuleFor(x => x.Scope)
            .NotEmpty()
            .When(x => !string.IsNullOrEmpty(x.Scope));
    }
}
