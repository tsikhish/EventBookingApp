using Data;
using Domain;
using Domain.Post;
using EventBookingApp.AppSettings;
using EventBookingApp.Controllers;
using EventBookingApp.Services;
using EventBookingApp.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class ApplicationUserTests
    {
        private readonly Mock<IUserServices> _userServicesMock;
        private readonly Mock<PersonContext> _dbContextMock;
        private readonly ApplicationUser _applicationUser;
        public ApplicationUserTests()
        {
            _userServicesMock = new Mock<IUserServices>();
            var dbContextOptions = new DbContextOptionsBuilder<PersonContext>()
                        .UseInMemoryDatabase("LastEventBookingApp")
                        .Options;
            _dbContextMock = new Mock<PersonContext>(dbContextOptions);
            _applicationUser = new ApplicationUser(_userServicesMock.Object, _dbContextMock.Object, Mock.Of<IOptions<AppSetting>>());
        }
        [Fact]
        public async void Register_NewUser_ReturnsOk()
        {
            //Arrange
            var registerPerson = FakeUser();
            _userServicesMock.Setup(x => x.Register(It.IsAny<UserRegistration>())).ReturnsAsync(new AppUser());
            //Act
            var result = await _applicationUser.RegisterUser(registerPerson) as OkObjectResult;
            //Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }
        [Fact]
        public async void RegisterAlreadyExistedPerson_ReturnsWrong()
        {
            //Arrange
            var existignsUsername =FakeUser();
            var existingUser = new AppUser { UserName = existignsUsername.UserName };
            _userServicesMock.Setup(service => service.Register(It.IsAny<UserRegistration>()))
                 .ThrowsAsync(new Exception("Already exists."));
            //Act
            var result = await _applicationUser.RegisterUser(FakeUser());
            //Arrange
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Already exists.", badRequest.Value);
        }
        [Fact]
        public async Task ValidateRegistration_InvalidUser_ExceptionThrownWithErrorMessage()
        {
            // Arrange
            var user = new UserRegistration
            {
                UserName = "",
                Password = "",
                Role = ""
            };
            var existingUser = new AppUser { UserName = user.UserName, Password = user.Password, Role = user.Role };
            _userServicesMock.Setup(service => service.Register(It.IsAny<UserRegistration>()))
                 .ThrowsAsync(new Exception("username should be filled, Password should be filled, Rule should be filled"));

            // Act
            var result = await _applicationUser.RegisterUser(FakeUser());

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("username should be filled, Password should be filled, Rule should be filled", badRequest.Value);
        }
    
    
    [Fact]
        public async void LoginExistingPerson_ReturnsOk()
        {
            //Arrange
            var loginExistingPerson = FakeLogin();
            _userServicesMock.Setup(x => x.Login(It.IsAny<LoginUser>())).ReturnsAsync("fake_token");
            //Act
            var result=await _applicationUser.LoginUser(loginExistingPerson);
            //Assert
            Assert.NotNull(result);
            var okobject=Assert.IsType<OkObjectResult>(result);
            Assert.Equal("fake_token",okobject.Value);
        }
        [Fact]
        public async void LoginNonExitingPerson_ReturnsWrong()
        {
            //Arrange
            var loginNonExistingPerson = FakeLogin();
            var expectedException = "Your Account doesnt exists or Password is not correct, please check it";
            _userServicesMock.Setup(x => x.Login(loginNonExistingPerson))
                .ThrowsAsync(new Exception(expectedException));
            //Act
            var result = await _applicationUser.LoginUser(loginNonExistingPerson);
            //Assert
            var badrequest= Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(expectedException, badrequest.Value);
        }
        [Fact]
        public async Task ValidateLogin_InvalidUser_ExceptionThrownWithErrorMessage()
        {
            // Arrange
            var user = new LoginUser
            {
                UserName = "",
                Password = "",
            };
            var existingUser = new AppUser { UserName = user.UserName, Password = user.Password };
            _userServicesMock.Setup(service => service.Login(It.IsAny<LoginUser>()))
                 .ThrowsAsync(new Exception("username should be filled, Password should be filled"));

            // Act
            var result = await _applicationUser.LoginUser(user);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("username should be filled, Password should be filled", badRequest.Value);
        }

        private LoginUser FakeLogin()
        {
            return new LoginUser
            {
                UserName = "tsikhish",
                Password = "tsikhish",
            };  
        }
        private UserRegistration FakeUser()
        {
            return new UserRegistration
            {
                UserName = "tsikhish",
                Password = "tsikhish",
                Role = "Accountant",
            };
        }
    }
}
