using Data;
using Domain;
using Domain.Post;
using EventBookingApp.AppSettings;
using EventBookingApp.Migrations;
using EventBookingApp.Services;
using EventBookingApp.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SendGrid.Helpers.Mail;
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
        private readonly IUserServices _userservice;
        private readonly Email _email;
        private readonly ILogger _logger;

        public ApplicationUser(Email email, IUserServices userservice, ILogger<ApplicationUser> logger)
        {
            _userservice = userservice;
            _email = email;
            _logger = logger;
        }
        [HttpPost("/applicationuser/registration")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistration user)
        {
            try
            {
                var newUser = await _userservice.Register(user);
                newUser.VerificationToken = Guid.NewGuid().ToString();
                await _userservice.UpdateUserAsync(newUser);
                var verificationLink = Url.Action(nameof(VerifyEmail), "ApplicationUser", new { token = newUser.VerificationToken }, Request.Scheme);
                var subject = "Welcome to Our Service";
                var body = $"<h1>Welcome, {newUser.UserName}!</h1><p>Thank you for registering.</p>";
                _logger.LogInformation($"Approving loan for {newUser.UserName}");
                await _email.SendEmailAsync(newUser.Email, subject, body);
                _logger.LogInformation($"Loan for {newUser.UserName} approved and email sent.");
                return Ok($"{newUser.UserName} registered successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("/applicationuser/verifyemail")]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            try
            {
                var user = await _userservice.GetUserByVerificationTokenAsync(token);
                if (user == null)
                {
                    _logger.LogWarning("Invalid verification token.");
                    return BadRequest("Invalid token.");
                }

                user.IsEmailVerified = true;
                await _userservice.UpdateUserAsync(user);

                return Ok("Email verified successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("/user/login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginUser loginUser)
        {
            try
            {
                var token = await _userservice.Login(loginUser);
                return Ok(token);
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}");
            }
        }
        [HttpPost("/user/forgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPassw request)
        {
            var user = await _userservice.GetUserByEmailAsync(request.Email);
            if(user == null)
            {
                return BadRequest("User not found.");
            }
            user.PasswordResetToken = Guid.NewGuid().ToString();
            user.ResetTokenExpiration = DateTime.UtcNow.AddHours(1);

            await _userservice.UpdateUserAsync(user);
            var resetLink = Url.Action(nameof(ResetPassword), "Request", new { token = user.PasswordResetToken }, Request.Scheme);
            var subject = "Password Reset Request";
            var body = $"<p>Please reset your password by clicking <a href=\"{resetLink}\">here</a>. The link is valid for 1 hour.</p>";
            await _email.SendEmailAsync(user.Email, subject, body);

            return Ok("Password reset link has been sent to your email.");
        }
        [HttpPost("/user/resetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] Domain.Post.ResetPassword resetPassword)
        {
            _logger.LogInformation("Attempting to send password reset email to {Email}", user.Email);

            try
            {
                var user =await _userservice.GetUserByVerificationTokenAsync(resetPassword.Token);
                if (user == null || user.ResetTokenExpiration < DateTime.UtcNow)
                {
                    return BadRequest("Invalid or expired token.");
                }
                if (resetPassword.NewPassword != resetPassword.ConfirmedPassword)
                {
                    return BadRequest("New password and confirmed password do not match.");
                }
                user.Password = BCrypt.Net.BCrypt.HashPassword(resetPassword.NewPassword);
                user.PasswordResetToken = null;
                user.ResetTokenExpiration = null;
                await _userservice.UpdateUserAsync(user);
                _logger.LogInformation("Password reset email sent successfully.");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending password reset email.");

            }
        }
    }
}
