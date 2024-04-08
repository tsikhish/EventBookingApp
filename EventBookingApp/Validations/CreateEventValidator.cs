using Domain.Post;
using FluentValidation;

namespace EventBookingApp.Validations
{
    public class CreateEventValidator:AbstractValidator<CreateEventDto>
    {
        public CreateEventValidator()
        {
            RuleFor(x=>x.EventName).NotEmpty().WithMessage("EventName should be filled");
            RuleFor(x => x.MaxBooking).NotEmpty().GreaterThan(0).WithMessage("MaxBooking should be filled and greater than 0");
            RuleFor(x => x.Location).NotEmpty().WithMessage("Location should be filled");
        }
    }
}
