using Data;
using Domain;
using Domain.Post;
using EventBookingApp.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventBookingApp.Services
{
    public interface IEventService
    {
        public Task<CreateEvent> CreateEvent(CreateEventDto createEventDto);
        public Task<string> DeleteEvent(string username);
        public  Task<CreateEvent> UpdateEvent(string username, CreateEventDto createEventDto);

    }
    public class EventService:IEventService
    {
        private readonly PersonContext _personContext;
        public EventService(PersonContext personContext)
        {
            _personContext=personContext;   
        }
        public async Task<CreateEvent> CreateEvent(CreateEventDto createEventDto)
        {
            await ValidateEvent(createEventDto);
            var existingEvent = await _personContext.CreateEvent.FirstOrDefaultAsync(x => x.EventName == createEventDto.EventName);
            if (existingEvent != null)
            {
                throw new System.Exception($"{existingEvent} already exists");
            }
            var newEvent = new CreateEvent
            {
                EventName = createEventDto.EventName,
                Location = createEventDto.Location,
                MaxBooking = createEventDto.MaxBooking,
                Created=DateTime.Now,
            };
            await _personContext.AddAsync(newEvent);
            await _personContext.SaveChangesAsync();
            return newEvent;
        }
        public async Task<string> DeleteEvent(string username)
        {
            var existingEvent =await _personContext.CreateEvent.FirstOrDefaultAsync(x => x.EventName == username);
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
            existingEvent.EventName = createEventDto.EventName;
            existingEvent.Location = createEventDto.Location;
            existingEvent.MaxBooking = createEventDto.MaxBooking;
            existingEvent.Created = DateTime.Now;
            _personContext.Update(existingEvent);
            _personContext.SaveChanges();
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
