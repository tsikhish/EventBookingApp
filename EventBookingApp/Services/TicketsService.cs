using Data;
using Domain;
using Domain.Post;
using EventBookingApp.Validations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EventBookingApp.Services
{
    public interface ITicketService
    {
        public Task<Tickets> TicketBooking(TicketDto ticket, string userId);
        public Task<string> TicketAvailability();
        public Task<Tickets> DeleteTickets(int id,string userId);

    }
    public class TicketsService:ITicketService
    {
        private readonly PersonContext _personContext;
        public TicketsService(PersonContext personContext)
        {
            _personContext=personContext;
        }
        public async Task<Tickets> TicketBooking(TicketDto ticket,string userId)
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
            if (existingPlace.Count> 0)
            {
                throw new InvalidOperationException($"Unfortunately your ticket {existingPlace} is already booked");
            }
            if (existingPlace.MaxBooking == 0)
            {
                throw new InvalidOperationException($"Tickets are sold out");
            }
            var ticketsForEvent = await _personContext.Tickets.Where(x => x.EventName == ticket.EventName).ToListAsync();
            foreach (var item in ticketsForEvent)
            {
                if (item.Id != existingPlace.Id)
                {
                    item.MaxBooking--;
                    _personContext.Update(item);
                }
            }
            existingPlace.AppUserId = userId;
            existingPlace.MaxBooking--;
            existingPlace.Count++;
            _personContext.Update(existingPlace);
            await _personContext.SaveChangesAsync();

            return existingPlace;
        }
        public async Task<string> TicketAvailability()
        {
            var availableTickets= await _personContext.Tickets.Where(x=>x.Count==0).ToListAsync();
            if(!availableTickets.Any())
            {
                throw new System.Exception($"Available tickets is not found");
            }
            var ticketInfoList = availableTickets.Select(t => $"Event: {t.EventName}, Tickets: {t.Id}");
            var ticketInfo = string.Join("\n", ticketInfoList);
            return ticketInfo;
        }
        public async Task<Tickets> DeleteTickets(int id,string userId)
        {
            var existingTickets = await _personContext.Tickets.FirstOrDefaultAsync(x => x.Id == id);
            if(existingTickets== null)
            {
                throw new SystemException($"{existingTickets} doesn't exist");
            }
            if (existingTickets.Count == 0)
            {
                throw new SystemException($"{existingTickets} was not booked and can not be cancelled");
            }
            if(existingTickets.AppUserId!=userId)
            {
                throw new SystemException($"{userId} can not cancel other persons tickets");
            }
            var ticketsForEvent = await _personContext.Tickets.Where(x => x.EventName == existingTickets.EventName).ToListAsync();
            foreach (var item in ticketsForEvent)
            {
                if (item.Id != existingTickets.Id)
                {
                    item.MaxBooking++;
                    _personContext.Update(item);
                }
            }
            existingTickets.MaxBooking++;
            existingTickets.Count--;
            existingTickets.AppUserId = null;
            _personContext.Update(existingTickets);
            _personContext.SaveChanges();
            return existingTickets;
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
