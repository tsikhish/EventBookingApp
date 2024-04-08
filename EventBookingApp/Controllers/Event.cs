using Data;
using Domain;
using Domain.Post;
using EventBookingApp.Services;
using EventBookingApp.Validations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public Event(PersonContext personcontext,IEventService eventService)
        {
            _personcontext = personcontext;
            _eventService = eventService;
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
                return BadRequest(ex.Message);
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
                return BadRequest(ex.Message);
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
                return BadRequest(ex.Message);
            }
        }
    }
}
