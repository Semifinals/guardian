namespace Semifinals.Guardian.DTOs;

public class OAuthGrantPasswordDto
{
    [JsonPropertyName("grant_type")]
    public string GrantType;

    [JsonPropertyName("username")]
    public string Username;

    [JsonPropertyName("password")]
    public string Password;

    [JsonPropertyName("scope")]
    public string? Scope;

    public OAuthGrantPasswordDto(
        string grantType,
        string username,
        string password,
        string? scope)
    {
        GrantType = grantType;
        Username = username;
        Password = password;
        Scope = scope;
    }
}

public class OAuthGrantPasswordDtoValidator
    : AbstractValidator<OAuthGrantPasswordDto>
{
    public OAuthGrantPasswordDtoValidator()
    {
        RuleFor(x => x.GrantType)
            .Equal("password");

        RuleFor(x => x.Username)
            .NotEmpty();

        RuleFor(x => x.Password)
            .NotEmpty();

        RuleFor(x => x.Scope)
            .NotEmpty()
            .When(x => !string.IsNullOrEmpty(x.Scope));
    }
}
