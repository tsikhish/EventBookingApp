using Data;
using Domain;
using Domain.Post;
using EventBookingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EventBookingApp.Controllers
{
    [Route("api/[controller]")]
    public class UserTickets : Controller
    {

        private readonly PersonContext _personContext;
        private readonly ITicketService _ticketService;
        public UserTickets(PersonContext personContext,ITicketService ticketService)
        {
            _personContext = personContext;
            _ticketService = ticketService;
        }
        [Authorize(Roles = Domain.Role.User)]
        [HttpPost("/events/{event-id}/book")]
        public async Task<IActionResult> TicketBooking(TicketDto tickets)
        {
            try
            {
                var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var newTickets = await _ticketService.TicketBooking(tickets, userId);
                return Ok(newTickets);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = Domain.Role.User)]
        [HttpGet("/events/{event_id}/tickets")]
        public async Task<IActionResult> AvailableTickets()
        {
            try
            {
                var availableTickets = await _ticketService.TicketAvailability();
                return Ok(availableTickets);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles= Domain.Role.User)]
        [HttpPost("/events/{event_id}/cancelbooking")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            try
            {
                var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var cancelBooking = await _ticketService.DeleteTickets(id, userId);
                return Ok(cancelBooking);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
