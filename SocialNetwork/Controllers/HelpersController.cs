using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialNetwork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelpersController : ControllerBase
    {
        [HttpGet("who-am-i")]
        [Authorize]
        public IActionResult WhoAmI()
        {
            var nameClaim = User.FindFirst(ClaimTypes.Name)?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRoleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userIdClaim == null)
                return Unauthorized();
            if(userRoleClaim == Role.Admin.ToString())
            {
                var admin = "Admin";
                return Ok(new {admin, nameClaim, userIdClaim, userRoleClaim });
            }
            return Ok(new { nameClaim, userIdClaim, userRoleClaim});
        }
    }
}
