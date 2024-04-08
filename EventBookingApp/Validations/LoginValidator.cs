using Domain.Post;
using FluentValidation;

namespace EventBookingApp.Validations
{
    public class LoginValidator:AbstractValidator<LoginUser>
    {
        public LoginValidator()
        {
            RuleFor(x=>x.UserName).NotEmpty().WithMessage("Username should be filled");
            RuleFor(x => x.Password).NotEmpty().WithMessage("password should be filled");
        }
    }
}
