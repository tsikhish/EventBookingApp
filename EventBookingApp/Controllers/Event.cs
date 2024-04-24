using Data;
using Domain;
using Domain.Post;
using EventBookingApp.Services;
using EventBookingApp.Validations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EventBookingApp.Controllers
{
    [Route("api/[controller]")]
    public class Event : Controller
    {
        private readonly PersonContext _personcontext;
        private readonly IEventService _eventService;
        private readonly ILogger<Event> _logger;
        public Event(PersonContext personcontext,IEventService eventService,ILogger<Event> logger)
        {
            _personcontext = personcontext;
            _eventService = eventService;
            _logger = logger;
        }

        [Authorize(Roles = Domain.Role.Accountant)]
        [HttpPost("/event/create")]
        public async Task<IActionResult> AdminCreateEvent(CreateEventDto createEventDto)
        {
            try
            {
                var newEvent = await _eventService.CreateEvent(createEventDto);
                return Ok(newEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request for event creation.");
                return BadRequest("An error occurred while processing your request. Please try again later.");
            }
        }
        [Authorize(Roles = Domain.Role.Accountant)]
        [HttpDelete("/event/delete")]
        public async Task<ActionResult<IEnumerable<CreateEvent>>> AdminDeleteEvent(string username)
        {
            try
            {
                var existingEvent = await _eventService.DeleteEvent(username);
                return Ok($"{existingEvent} deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request for deleting event.");
                return BadRequest("An error occurred while processing your request. Please try again later.");
            }
        }
        [Authorize(Roles = Domain.Role.Accountant)]

        [HttpPut("/event/update")]
        public async Task<ActionResult<IEnumerable<CreateEvent>>> AdminUpdatesEvent(string username,CreateEventDto createEvent)
        {
            try
            {
                var existingEvent = await _eventService.UpdateEvent(username, createEvent);
                return Ok(existingEvent);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request for updating event.");
                return BadRequest("An error occurred while processing your request. Please try again later.");

            }
        }
        [AllowAnonymous]
        [HttpGet("/events")]
        public async Task<IActionResult> SearchAnyEvent(string EventName)
        {
            try
            {
                var existingEvent = await _eventService.SearchEvent(EventName);
                return Ok(existingEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request for searching event.");
                return BadRequest("An error occurred while processing your request. Please try again later.");

            }
        }
    }
}
