using FluentValidation;
using UserManagement.Application.DTOs.Requests;

namespace UserManagement.Application.Validators;

public class RegisterUserValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MinimumLength(3).Must(x => !IsReserved(x)).WithMessage("Username is reserved or too short.");
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Phone).NotEmpty();
        RuleFor(x => x.FullName).NotEmpty().Length(2, 80);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(12)
            .Matches("[A-Z]").Matches("[a-z]").Matches("[0-9]").Matches("[^a-zA-Z0-9]")
            .WithMessage("Password must be at least 12 chars and contain 3 of 4 categories.");
        RuleFor(x => x.TermsVersion).GreaterThan(0).WithMessage("You must accept terms.");
    }

    private static bool IsReserved(string username)
    {
        var reserved = new[] { "admin", "support", "system", "root" };
        return reserved.Contains(username.ToLower());
    }
}
