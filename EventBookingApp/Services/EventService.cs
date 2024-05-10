using Data;
using Domain;
using Domain.Post;
using EventBookingApp.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace EventBookingApp.Services
{
    public interface IEventService
    {
        public Task<CreateEvent> CreateEvent(CreateEventDto createEventDto);
        public Task<string> DeleteEvent(string username);
        public Task<CreateEvent> UpdateEvent(string username, CreateEventDto createEventDto);
        public Task<CreateEvent> SearchEvent(string EventName);
    }
    public class EventService : IEventService
    {
        private readonly PersonContext _personContext;
        public EventService(PersonContext personContext)
        {
            _personContext = personContext;
        }
        public async Task<CreateEvent> CreateEvent(CreateEventDto createEventDto)
        {
            await ValidateEvent(createEventDto);
            var existingEvent = await _personContext.CreateEvent.FirstOrDefaultAsync(x => x.EventName == createEventDto.EventName);
            if (existingEvent != null)
            {
                throw new System.Exception($"{existingEvent} already exists");
            }
            var eventTime = await TimeOfEvent(createEventDto);
            if (existingEvent.Location == createEventDto.Location && existingEvent.EventTime == eventTime)
            {
                throw new System.Exception($"Someone has already booked an event here");
            }
            var newEvent = new CreateEvent
            {
                EventName = createEventDto.EventName,
                Location = createEventDto.Location,
                MaxBooking = createEventDto.MaxBooking,
                Created = DateTime.Now,
                EventTime = eventTime
            };
            await _personContext.AddAsync(newEvent);
            await _personContext.SaveChangesAsync();
            if (createEventDto.MaxBooking > 0)
            {
                for (int i = 0; i < createEventDto.MaxBooking; i++)
                {
                    var availableTickets = new Tickets
                    {
                        EventId = newEvent.Id,
                        EventName = newEvent.EventName,
                        Count = 0,
                        MaxBooking = newEvent.MaxBooking,
                        TimeOfEvent = eventTime
                    };
                    _personContext.Tickets.Add(availableTickets);
                }
            }
            await _personContext.SaveChangesAsync();
            return newEvent;
        }
        public async Task<string> DeleteEvent(string username)
        {
            var existingEvent = await _personContext.CreateEvent.FirstOrDefaultAsync(x => x.EventName == username);
            if (existingEvent == null)
            {
                throw new System.Exception($"{existingEvent.EventName} doesnt exists");
            }
            _personContext.CreateEvent.Remove(existingEvent);
            _personContext.SaveChanges();
            return existingEvent.EventName;
        }
        public async Task<CreateEvent> UpdateEvent(string username, CreateEventDto createEventDto)
        {
            await ValidateEvent(createEventDto);
            var existingEvent = await _personContext.CreateEvent.FirstOrDefaultAsync(x => x.EventName == username);
            if (existingEvent == null)
                throw new System.Exception($"{existingEvent} doesnt exists");
            var eventTime = await TimeOfEvent(createEventDto);
            existingEvent.EventName = createEventDto.EventName;
            existingEvent.Location = createEventDto.Location;
            existingEvent.MaxBooking = createEventDto.MaxBooking;
            existingEvent.EventTime = eventTime;
            existingEvent.Created = DateTime.Now;
            _personContext.Update(existingEvent);
            _personContext.SaveChanges();
            return existingEvent;
        }
        private static Task<DateTime> TimeOfEvent(CreateEventDto createEventDto)
        {
            int year = createEventDto.YearOfEvent;
            int month = createEventDto.MonthOfEvent;
            int day = createEventDto.DayOfEvent;
            int hour = createEventDto.HoursOfEvent;
            DateTime eventTime = new DateTime(year, month, day, hour, 0, 0);
            return Task.FromResult(eventTime);
        }
        public async Task<CreateEvent> SearchEvent(string EventName)
        {
            var existingEvent = await _personContext.CreateEvent.FirstOrDefaultAsync(x => x.EventName == EventName);
            if (existingEvent == null)
            {
                throw new System.Exception($"{existingEvent} doesnt exist");
            }
            return existingEvent;
        }
        private async Task ValidateEvent(CreateEventDto createEventDto)
        {
            var validator = new CreateEventValidator();
            var validationResult = await validator.ValidateAsync(createEventDto);
            var errorMessage = "";
            if (!validationResult.IsValid)
            {
                foreach (var claim in validationResult.Errors)
                {
                    errorMessage += claim.ErrorMessage + " , ";
                }
                throw new System.Exception(errorMessage);
            }
        }
    }
}
