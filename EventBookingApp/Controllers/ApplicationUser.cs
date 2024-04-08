using Data;
using Domain.Post;
using EventBookingApp.AppSettings;
using EventBookingApp.Services;
using EventBookingApp.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
namespace EventBookingApp.Controllers
{
    [Route("api/[controller]")]
    public class ApplicationUser : Controller
    {
        private readonly PersonContext _personcontext;
        private readonly AppSetting _appsettings;
        private readonly IUserServices _userservice;
        public ApplicationUser(IUserServices userservice,PersonContext personcontext, IOptions<AppSetting> appsettings)
        {
            _personcontext=personcontext;
            _appsettings=appsettings.Value;
            _userservice=userservice;
        }
        [HttpPost("/applicationuser/registration")]
        public async Task<IActionResult> RegisterUser([FromBody]UserRegistration user)
        {
            try
            {
               var newUser=await _userservice.Register(user);
               return Ok($"{newUser.UserName} registered successfully");
            }
            catch(Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }
        [HttpPost("/user/login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginUser loginUser)
        {
            try
            {
                var token = await _userservice.Login(loginUser);
                var existingPerson = await _personcontext.AppUser.FirstOrDefaultAsync(x => x.UserName == loginUser.UserName);
                if (token != null)
                {
                    return Ok(
                        new
                        {
                            Id = existingPerson.Id,
                            Username = existingPerson.UserName,
                            Role = existingPerson.Role,
                            Token = token,
                        }
                        );
                }
                else
                {
                    return Unauthorized($"{loginUser.UserName} is not authorized");
                }
            }
            catch(Exception ex)
            {
                return BadRequest($"{ex.Message}");
            }
        }
    }
}
