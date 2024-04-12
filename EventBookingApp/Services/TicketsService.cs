using Data;
using Domain;
using Domain.Post;
using EventBookingApp.Validations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EventBookingApp.Services
{
    public interface ITicketService
    {
        public Task<Tickets> TicketBooking(TicketDto ticket);

    }
    public class TicketsService:ITicketService
    {
        private readonly PersonContext _personContext;
        public TicketsService(PersonContext personContext)
        {
            _personContext=personContext;
        }
        public async Task<Tickets> TicketBooking(TicketDto ticket)
        {
            var existingEvent = await _personContext.CreateEvent.FirstOrDefaultAsync(x => x.EventName == ticket.EventName);
            if (existingEvent == null)
            {
                throw new ArgumentException($"{existingEvent} doesnt exists");
            }
            var existingPlace = await _personContext.Tickets.FirstOrDefaultAsync(x => x.Id == ticket.DesiredTicket);
            if(existingPlace == null || existingPlace.EventName!=ticket.EventName) 
            {
                throw new ArgumentException($"{existingPlace} doesnt exists");
            }
            if (existingPlace.Count!= 0)
            {
                throw new InvalidOperationException($"Unfortunately your ticket {existingPlace} is already booked");
            }
            if (existingPlace.MaxBooking == 0)
            {
                throw new InvalidOperationException($"Tickets are sold out");
            }
            var ticketsForEvent = await _personContext.Tickets.Where(x => x.EventName == ticket.EventName).ToListAsync();
            foreach (var ticketForEvent in ticketsForEvent)
            {
                ticketForEvent.MaxBooking = existingPlace.MaxBooking--;
            }
            existingPlace.Count++;
            _personContext.UpdateRange(ticketsForEvent);
            _personContext.Update(existingPlace);
            await _personContext.SaveChangesAsync();

            return existingPlace;
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
