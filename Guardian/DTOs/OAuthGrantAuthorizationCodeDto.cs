namespace Semifinals.Guardian.DTOs;

public class OAuthGrantAuthorizationCodeDto
{
    [JsonPropertyName("grant_type")]
    public string GrantType;

    [JsonPropertyName("code")]
    public string Code;

    [JsonPropertyName("client_id")]
    public string ClientId;

    [JsonPropertyName("redirect_uri")]
    public string? RedirectUri;

    public OAuthGrantAuthorizationCodeDto(
        string grantType,
        string code,
        string clientId,
        string? redirectUri)
    {
        GrantType = grantType;
        Code = code;
        ClientId = clientId;
        RedirectUri = redirectUri;
    }
}

public class OAuthGrantAuthorizationCodeDtoValidator
    : AbstractValidator<OAuthGrantAuthorizationCodeDto>
{
    public OAuthGrantAuthorizationCodeDtoValidator()
    {
        RuleFor(x => x.GrantType)
            .Equal("authorization_code");

        RuleFor(x => x.Code)
            .NotEmpty();

        RuleFor(x => x.ClientId)
            .NotEmpty();

        RuleFor(x => x.RedirectUri)
            .Must(x => Uri.TryCreate(x, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrEmpty(x.RedirectUri));
    }
}
