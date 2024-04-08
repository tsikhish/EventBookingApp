using Domain.Post;
using FluentValidation;

namespace EventBookingApp.Validations
{
    public class Registration:AbstractValidator<UserRegistration>
    {
        public Registration()
        {
            RuleFor(x => x.Role).NotEmpty().WithMessage("Rule should be filled");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password should be filled");
            RuleFor(x => x.UserName).NotEmpty().WithMessage("username should be filled");
        }
    }
}
