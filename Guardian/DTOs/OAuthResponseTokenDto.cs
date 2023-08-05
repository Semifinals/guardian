namespace Semifinals.Guardian.DTOs;

public class OAuthResponseTokenDto
{
    [JsonPropertyName("response_type")]
    public string ResponseType { get; set; } = null!;

    [JsonPropertyName("client_id")]
    public string ClientId { get; set; } = null!;

    [JsonPropertyName("redirect_uri")]
    public string? RedirectUri { get; set; }

    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    [JsonPropertyName("state")]
    public string? State { get; set; }
}

public class OAuthResponseTokenDtoValidator
    : AbstractValidator<OAuthResponseTokenDto>
{
    public OAuthResponseTokenDtoValidator()
    {
        RuleFor(x => x.ResponseType)
            .Equal("token");

        RuleFor(x => x.ClientId)
            .NotEmpty();

        RuleFor(x => x.RedirectUri)
            .Must(x => Uri.TryCreate(x, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrEmpty(x.RedirectUri));

        RuleFor(x => x.Scope)
            .NotEmpty()
            .When(x => !string.IsNullOrEmpty(x.Scope));

        RuleFor(x => x.State)
            .NotEmpty()
            .When(x => !string.IsNullOrEmpty(x.State));
    }
}
