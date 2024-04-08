using Domain.Post;
using FluentValidation;

namespace EventBookingApp.Validations
{
    public class TicketValidator:AbstractValidator<TicketDto>
    {
        public TicketValidator()
        {
            RuleFor(x=>x.EventName).NotEmpty().WithMessage("eventname should be filled");
            RuleFor(x => x.BookedTicketId).GreaterThan(0).WithMessage("BokkedTicked should be greater than 0")
                .NotEmpty().WithMessage("bookedTicket should be filled");
            RuleFor(x => x.currentBookint).GreaterThan(0).WithMessage("currentBookint should be greater than 0")
                            .NotEmpty().WithMessage("currentBookint should be filled");
        }
    }
}
