using Microsoft.AspNetCore.Mvc;
using Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Project_Diet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly EmailService _emailService;

        public EmailController(EmailService emailService)
        {
            _emailService = emailService;
        }

        // POST api/<EmailController>
        [HttpPost("SendEmail")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequest request)
        {
            await _emailService.SendEmailAsync(request.ToEmail, request.ToName, request.Subject, request.Body);
            return Ok(new { message = "Email sent successfully!" });
        }
    }
}
