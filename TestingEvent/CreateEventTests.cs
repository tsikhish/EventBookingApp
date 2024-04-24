using Castle.Core.Logging;
using Data;
using EventBookingApp.AppSettings;
using EventBookingApp.Controllers;
using EventBookingApp.Migrations;
using EventBookingApp.Services;
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
            var dbContextMock=new Mock<PersonContext>(dbContextOptions);
            _event=new Event(dbContextMock.Object, _mock.Object,Mock.Of<ILogger<Event>>());
        }
    }
}
