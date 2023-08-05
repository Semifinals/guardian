namespace Semifinals.Guardian.DTOs;

public class OAuthGrantPasswordDto
{
    [JsonPropertyName("grant_type")]
    public string GrantType { get; set; } = null!;

    [JsonPropertyName("username")]
    public string Username { get; set; } = null!;

    [JsonPropertyName("password")]
    public string Password { get; set; } = null!;

    [JsonPropertyName("scope")]
    public string? Scope { get; set; }
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
