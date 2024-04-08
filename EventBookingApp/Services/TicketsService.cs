using Data;
using Domain;
using Domain.Post;
using EventBookingApp.Validations;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace EventBookingApp.Services
{
    public interface ITicketService
    {
        public Task<Tickets> TicketBooking(int bookedtkt, TicketDto ticketDto);

    }
    public class TicketsService:ITicketService
    {
        private readonly PersonContext _personContext;
        public TicketsService(PersonContext personContext)
        {
            _personContext=personContext;
        }
        public async Task<Tickets> TicketBooking(int bookedtkt,TicketDto ticketDto)
        {
            await ValidateTickets(ticketDto);
            var existingEvent = await _personContext.CreateEvent.FirstOrDefaultAsync(x => x.EventName == ticketDto.EventName);
            if (existingEvent == null)
            {
                throw new System.Exception($"{existingEvent} doesnt exists");
            }
            var bookedticket= await _personContext.Tickets.FirstOrDefaultAsync(x => x.BookedTickedId == bookedtkt);
            if(bookedticket != null)
            {
                throw new System.Exception($"Unfortunately your ticket {bookedticket} is already booked");
            }
            var availableTickets = existingEvent.MaxBooking;
            if (availableTickets == 0)
            {
                throw new System.Exception($"Tickets are sold out");
            }
            var newTickets = new Tickets
            {
                BookedTickedId= bookedtkt,
                EventId = existingEvent.Id,
                EventName = existingEvent.EventName,
                Count = availableTickets--,
            };
            await _personContext.Tickets.AddAsync(newTickets);
            await _personContext.SaveChangesAsync();
            return newTickets;
        }
        private async Task ValidateTickets(TicketDto ticketDto)
        {
            var validator = new TicketValidator();
            var validationResult=await validator.ValidateAsync(ticketDto);
            var errormessage = "";
            if (!validationResult.IsValid)
            {
                foreach(var item in  validationResult.Errors)
                {
                    errormessage += item.ErrorMessage+" , ";
                }
                throw new System.Exception(errormessage);
            }
        }
    }
}
