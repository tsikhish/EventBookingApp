using Domain.Post;
using FluentValidation;

namespace EventBookingApp.Validations
{
    public class TicketValidator:AbstractValidator<TicketDto>
    {
        public TicketValidator()
        {
            RuleFor(x=>x.EventName).NotEmpty().WithMessage("eventname should be filled");
        }
    }
}
