using Castle.Core.Logging;
using Data;
using Domain.Post;
using Domain;
using EventBookingApp.AppSettings;
using EventBookingApp.Controllers;
using EventBookingApp.Migrations;
using EventBookingApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;
using Xunit.Abstractions;
namespace TestingEvent
{
    public class CreateEventTests
    {
        private readonly Mock<IEventService> _mock;
        private readonly Event _event;
        public CreateEventTests()
        {
            _mock = new Mock<IEventService>();
            var dbContextOptions = new DbContextOptionsBuilder<PersonContext>()
                .UseInMemoryDatabase(databaseName: "LastEventBookingApp")
                .Options;
            var dbContextMock = new Mock<PersonContext>(dbContextOptions);
            _event = new Event(dbContextMock.Object, _mock.Object, Mock.Of<ILogger<Event>>());
        }
        [Fact]
        public async void ValidateEvent_ThrowsException()
        {
            // Arrange
            var user = new CreateEventDto
            {
                HoursOfEvent = 25,
                MaxBooking = -5,
                Location = "",
                EventName = ""
            };
            var existingUser = new CreateEvent
            {
                MaxBooking = user.MaxBooking,
                EventName = user.EventName,
                Location = user.Location,
                EventTime = DateTime.Now.AddHours(user.HoursOfEvent)
            };
            var errorMessage = "EventName should be filled, MaxBooking should be filled and greater than 0, Location should be filled, Hours should be filled, Hours should be from 0 to 24";

            _mock.Setup(service => service.CreateEvent(It.IsAny<CreateEventDto>()))
                 .ThrowsAsync(new Exception(errorMessage));

            // Act
            var result = await _event.AdminCreateEvent(user);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(errorMessage, badRequest.Value);
        }
        [Fact]
        public async void CreateExistingEvent_ThrowsException()
        {
            //Arrange
            var existingEvent = new CreateEventDto { EventName = "Scorpions", Location = "Tbilisi" };
            var existingUser = new CreateEvent { EventName = existingEvent.EventName, Location = existingEvent.Location };
            _mock.Setup(service => service.CreateEvent(It.IsAny<CreateEventDto>()))
                 .ThrowsAsync(new Exception("Someone has already booked an event here"));
            //Act
            var result = await _event.AdminCreateEvent(existingEvent);
            //Arrange
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Someone has already booked an event here", badRequest.Value);
        }

        [Fact]
        public async void CreateNonExistingEvent_ReturnsOk()
        {
            //Arrange
            var createevent = new CreateEventDto { EventName = "Scorpions", Location = "Tbilisi" }; ;
            _mock.Setup(x => x.CreateEvent(It.IsAny<CreateEventDto>())).ReturnsAsync(new CreateEvent());
            //Act
            var result = await _event.AdminCreateEvent(createevent) as OkObjectResult;
            //Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }
        [Fact]
        public async void DeleteExistingEvent_ThrowsException()
        {
            //Arrange
            var eventName = "ExistingEvent";
            var existingEvent = new CreateEvent { EventName = eventName };
            _mock.Setup(x => x.DeleteEvent(It.IsAny<string>())).ReturnsAsync(existingEvent.EventName);
            //Act
            var result = await _event.AdminDeleteEvent(eventName);
            //Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal($"{existingEvent.EventName} deleted successfully", okResult.Value);
        }
        [Fact]
        public async void DeleteNonExistingEvent_ThrowsException()
        {
            //Arrange
            var eventName = "ExistingEvent";
            var expectedException = $"{eventName} doesnt exists";
            _mock.Setup(x => x.DeleteEvent(eventName)).ThrowsAsync(new Exception(expectedException));
            //Act
            var result = await _event.AdminDeleteEvent(eventName);
            var badRequestResult = result.Result as BadRequestObjectResult;

            //Assert
            Assert.NotNull(badRequestResult);
            Assert.Equal(expectedException, badRequestResult.Value);
        }
        [Fact]
        public async void UpdateEventValidation_ThrowsException()
        {
            // Arrange
            var username = "ExistingEvent";
            var createEventDto = new CreateEventDto()
            {
                HoursOfEvent = 25,
                MaxBooking = -5,
                Location = "",
                EventName = ""
            };
            var existingUser = new CreateEvent
            {
                MaxBooking = createEventDto.MaxBooking,
                EventName = createEventDto.EventName,
                Location = createEventDto.Location,
                EventTime = DateTime.Now.AddHours(createEventDto.HoursOfEvent)
            };
            var errorMessage = "EventName should be filled, MaxBooking should be filled and greater than 0, Location should be filled, Hours should be filled, Hours should be from 0 to 24";
            _mock.Setup(service => service.UpdateEvent(It.IsAny<string>(), It.IsAny<CreateEventDto>()))
                    .ThrowsAsync(new Exception(errorMessage));
            //Act
            var result = await _event.AdminUpdatesEvent(username, createEventDto);
            var badRequestResult = result.Result as BadRequestObjectResult;

            //Assert
            Assert.NotNull(badRequestResult);
            Assert.Equal(errorMessage, badRequestResult.Value);

        }
        //[Fact]
        //public async void UpdateNonExistingEvent_ThrowsException()
        //{
        //    //Arrange
        //    var update = "Scorpions";
        //    var createEventDto = new CreateEventDto { EventName = "Imagine Dragons", Location = "Tbilisi" };
        //    _mock.Setup(service => service.UpdateEvent(It.IsAny<string>(), It.IsAny<CreateEventDto>()))
        //            .ThrowsAsync(new Exception($"{createEventDto} doesnt exists"));
        //    //Act
        //    _mock.Setup(x => x.UpdateEvent.FirstOrDefaultAsync(It.IsAny<Expression<Func<CreateEvent, bool>>>()))
        //         .ReturnsAsync((CreateEvent)null);

        //    //Arrange
        //    var createevent = new CreateEventDto { EventName = "Scorpions", Location = "Tbilisi" }; ;
        //    _mock.Setup(x => x.CreateEvent(It.IsAny<CreateEventDto>())).ReturnsAsync(new CreateEvent());
        //    //Act
        //    var result = await _event.AdminCreateEvent(createevent) as OkObjectResult;
        //    //Assert
        //    Assert.NotNull(result);
        //    Assert.Equal(200, result.StatusCode);
        //}
    }
}
