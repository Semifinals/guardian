namespace Semifinals.Guardian.DTOs;

public class OAuthGrantAuthorizationCodeDto
{
    [JsonPropertyName("grant_type")]
    public string GrantType { get; set; } = null!;

    [JsonPropertyName("code")]
    public string Code { get; set; } = null!;

    [JsonPropertyName("client_id")]
    public string ClientId { get; set; } = null!;

    [JsonPropertyName("redirect_uri")]
    public string? RedirectUri { get; set; }
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
