using Data;
using Domain;
using Domain.Post;
using EventBookingApp.AppSettings;
using EventBookingApp.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace EventBookingApp.Services
{
    public interface IUserServices
    {
        public Task<AppUser> Register([FromBody] UserRegistration user);
        public Task<string> Login([FromBody] LoginUser loginUser);
    }

    public class UserService : IUserServices
    {
        private readonly PersonContext _personcontext;
        private readonly AppSetting _appsetting;
        public UserService(PersonContext personcontext, IOptions<AppSetting> appsetting)
        {
            _personcontext = personcontext;
            _appsetting = appsetting.Value;
        }

        public async Task<AppUser> Register([FromBody] UserRegistration user)
        {
            await ValidateRegistration(user);
            var existingUser = await _personcontext.AppUser.FirstOrDefaultAsync(x => x.UserName == user.UserName);
            if (existingUser != null)
            {
                throw new Exception($"Already exists.");
            }
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
            var newUser = new Domain.AppUser
            {
                UserName = user.UserName,
                Password = hashedPassword,
                Role = user.Role,
            };
            await _personcontext.AppUser.AddAsync(newUser);
            await _personcontext.SaveChangesAsync();
            return newUser;
        }

        public async Task<string> Login([FromBody] LoginUser loginUser)
        {
            await ValidateLogin(loginUser);
            var token = await GenerateToken(loginUser);
            return token;
        }
        private async Task<string> GenerateToken([FromBody] LoginUser loginuser)
        {
            var existingPerson = await _personcontext.AppUser.FirstOrDefaultAsync(x => x.UserName == loginuser.UserName);
            if (existingPerson == null || !BCrypt.Net.BCrypt.Verify(loginuser.Password, existingPerson.Password))
            {
                throw new SystemException($"Your Account doesnt exists or Password is not correct, please check it");
            }

            var authClaims = new List<Claim>()
            {
               new Claim(ClaimTypes.NameIdentifier, existingPerson.Id.ToString()),
               new Claim(ClaimTypes.Name,existingPerson.UserName),
               new Claim(ClaimTypes.Role, existingPerson.Role),
            };
            var key = Encoding.ASCII.GetBytes(_appsetting.Secret);
            var authSecret = new SymmetricSecurityKey(key);
            var tokenObject = new JwtSecurityToken(
                expires: DateTime.Now.AddDays(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256));
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.WriteToken(tokenObject);
            return token;
        }
        private async Task ValidateRegistration([FromBody] UserRegistration user)
        {
            var validator = new Registration();
            var valid = await validator.ValidateAsync(user);
            var errorMessage = "";
            if (!valid.IsValid)
            {
                foreach (var item in valid.Errors)
                {
                    errorMessage += item.ErrorMessage + " , ";
                }
                throw new System.Exception(errorMessage);
            }
        }
        private async Task ValidateLogin([FromBody] LoginUser loginuser)
        {
            var validator = new LoginValidator();
            var valid = await validator.ValidateAsync(loginuser);
            var errorMessage = "";
            if (!valid.IsValid)
            {
                foreach (var item in valid.Errors)
                {
                    errorMessage += item.ErrorMessage + " , ";
                }
                throw new System.Exception(errorMessage);
            }
        }

    }
}

