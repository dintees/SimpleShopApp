using FluentValidation;

namespace SimpleShopApp.Models.Validators
{
    public class UserValidator : AbstractValidator<UserModel>
    {
        public UserValidator()
        {
            RuleFor(u => u.Username).NotEmpty().WithMessage("Username could not be empty");
            RuleFor(u => u.Password).NotEmpty().WithMessage("Password could not be empty").MinimumLength(8).WithMessage("Password must be at least 8 characters long");
            RuleFor(u => u.Email).NotEmpty().WithMessage("Email could not be empty").EmailAddress();
        }
    }
}
