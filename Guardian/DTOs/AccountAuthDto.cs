using System.Text.RegularExpressions;

namespace Semifinals.Guardian.DTOs;

public class AccountAuthDto
{
    [JsonPropertyName("emailAddress")]
    public string EmailAddress { get; set; } = null!;

    [JsonPropertyName("password")]
    public string Password { get; set; } = null!;
}

public class AccountAuthDtoValidator : AbstractValidator<AccountAuthDto>
{
    public AccountAuthDtoValidator()
    {
        RuleFor(x => x.EmailAddress)
            .NotNull()
            .WithMessage("A valid email address must be provided")
            .EmailAddress()
            .WithMessage("A valid email address must be provided");

        RuleFor(x => x.Password)
            .NotNull()
            .WithMessage("A valid password must be provided");

        When(x => x.Password is not null, () => RuleFor(x => x.Password)
            .Must(password => new Regex("[A-Z]").IsMatch(password))
            .WithMessage("The password must contain a capital letter")
            .Must(password => new Regex("[a-z]").IsMatch(password))
            .WithMessage("The password must contain a lower case letter")
            .Must(password => new Regex("[0-9]").IsMatch(password))
            .WithMessage("The password must contain a number")
            .Must(password => new Regex("[^A-Za-z0-9]").IsMatch(password))
            .WithMessage("The password must contain a symbol"));
    }
}
