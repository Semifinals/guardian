namespace Semifinals.Guardian.DTOs;

public class OAuthTokenDto
{
    [JsonPropertyName("grant_type")]
    public string? GrantType { get; }

    [JsonPropertyName("response_type")]
    public string? ResponseType { get; }

    public OAuthTokenDto(string? grantType, string? responseType)
    {
        GrantType = grantType;
        ResponseType = responseType;
    }
}

public class OAuthTokenDtoValidator : AbstractValidator<OAuthTokenDto>
{
    public OAuthTokenDtoValidator()
    {
        string[] responseTypes = new string[]
        {
            "code",
            "token"
        };
        
        string[] grantTypes = new string[]
        {
            "authorization_code",
            "password",
            "client_credentials",
            "refresh_token"
        };

        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.ResponseType) ^ !string.IsNullOrEmpty(x.GrantType))
            .WithName("grant_type") // Refactor to elsewhere?
            .WithMessage("A response_type or grant_type must be provided");
        
        RuleFor(x => x.ResponseType)
            .Must(x => responseTypes.Any(type => type == x))
            .When(x => string.IsNullOrEmpty(x.GrantType))
            .WithMessage("Invalid response_type provided");

        RuleFor(x => x.GrantType)
            .Must(x => grantTypes.Any(type => type == x))
            .When(x => string.IsNullOrEmpty(x.ResponseType))
            .WithMessage("Invalid grant_type provided");
    }
}
