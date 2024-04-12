using Data;
using Domain;
using Domain.Post;
using EventBookingApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [HttpPost()]
        [HttpPost("/events/{event-id}/book")]
        public async Task<IActionResult> TicketBooking(TicketDto tickets)
        {
            var newTickets =await _ticketService.TicketBooking(tickets);
            return Ok(newTickets);
        }

    }
}
