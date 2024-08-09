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
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Linq;

namespace EventBookingApp.Services
{
    public interface IUserServices
    {
        public Task<AppUser> Register([FromBody] UserRegistration user);
        public Task<string> Login([FromBody] LoginUser loginUser);
        Task<AppUser> GetUserByVerificationTokenAsync(string token);
        Task UpdateUserAsync(AppUser user);
        Task<AppUser> GetUserByEmailAsync(string email);

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
        public async Task<AppUser> GetUserByEmailAsync(string email)
        {
            return await _personcontext.AppUser.SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task UpdateUserAsync(AppUser user)
        {
            _personcontext.AppUser.Update(user);
            await _personcontext.SaveChangesAsync();
        }


        public async Task<AppUser> GetUserByVerificationTokenAsync(string token)
        {
            return await _personcontext.AppUser.FirstOrDefaultAsync(u => u.VerificationToken == token);
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
                Email = user.Email,
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
        private async Task<string> GenerateToken([FromBody] LoginUser loginUser)
        {
            AppUser appUser = null;
            bool isEmail = IsEmailFormat(loginUser.UserNameOrEmail);
            if (isEmail)
            {
                appUser = await _personcontext.AppUser.FirstOrDefaultAsync(x => x.Email == loginUser.UserNameOrEmail);
            }
            else
            {
                appUser = await _personcontext.AppUser.FirstOrDefaultAsync(x => x.UserName == loginUser.UserNameOrEmail);
            }
            if (appUser == null || !BCrypt.Net.BCrypt.Verify(loginUser.Password, appUser.Password))
            {
                throw new Exception("Your account doesn't exist or the password is incorrect, please check it.");
            }
            return GenerateToken(appUser);
        }
        public async Task ValidateRegistration([FromBody] UserRegistration user)
        {
            var validator = new Registration();
            var valid = await validator.ValidateAsync(user);
            if (!valid.IsValid)
            {
                var errorMessage = string.Join(", ", valid.Errors.Select(e => e.ErrorMessage));
                throw new System.Exception(errorMessage);
            }
        }
        
        public async Task ValidateLogin([FromBody] LoginUser loginuser)
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
        private bool IsEmailFormat(string input)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(input);
                return addr.Address == input;
            }
            catch
            {
                return false;
            }
        }
        private string GenerateToken(AppUser appUser)
        {
            var authClaims = new List<Claim>()
            {
               new Claim(ClaimTypes.NameIdentifier, appUser.Id.ToString()),
               new Claim(ClaimTypes.Name,appUser.UserName),
               new Claim(ClaimTypes.Role, appUser.Role),
            };
            var key = Encoding.ASCII.GetBytes(_appsetting.Secret);
            var authSecret = new SymmetricSecurityKey(key);
            var tokenObject = new JwtSecurityToken(
                expires: DateTime.Now.AddDays(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256));
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(tokenObject);
        }
    }
}

