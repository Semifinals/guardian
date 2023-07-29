namespace Semifinals.Guardian.DTOs;

public class OAuthResponseTokenDto
{
    [JsonPropertyName("response_type")]
    public string ResponseType;

    [JsonPropertyName("client_id")]
    public string ClientId;

    [JsonPropertyName("redirect_uri")]
    public string? RedirectUri;

    [JsonPropertyName("scope")]
    public string? Scope;

    [JsonPropertyName("state")]
    public string? State;

    public OAuthResponseTokenDto(
        string responseType,
        string clientId,
        string? redirectUri,
        string? scope,
        string? state)
    {
        ResponseType = responseType;
        ClientId = clientId;
        RedirectUri = redirectUri;
        Scope = scope;
        State = state;
    }

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
