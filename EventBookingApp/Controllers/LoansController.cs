using Domain.Post;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace EventBookingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoansController : ControllerBase
    {
        private readonly Email _emailService;
        private readonly ILogger _logger;
        public LoansController(Email emailService, ILogger<LoansController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        [HttpPost("approve")]
        public async Task<IActionResult> ApproveLoan(int loanId)
        {
            _logger.LogInformation($"Approving loan with ID {loanId}.");
            await _emailService.SendEmailAsync("user@example.com", "Loan Approved", $"Your loan with ID {loanId} has been approved!");
            _logger.LogInformation($"Loan with ID {loanId} approved and email sent.");

            return Ok(new { Message = $"Loan with ID {loanId} approved." });
        }
    }

}
