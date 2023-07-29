namespace Semifinals.Guardian.DTOs;

public class OAuthGrantClientCredentialsDto
{
    [JsonPropertyName("grant_type")]
    public string GrantType;

    [JsonPropertyName("scope")]
    public string? Scope;

    public OAuthGrantClientCredentialsDto(
        string grantType,
        string? scope)
    {
        GrantType = grantType;
        Scope = scope;
    }
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
