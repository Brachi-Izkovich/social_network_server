using Microsoft.AspNetCore.Mvc;
using Common.Dto;
using Service.Interfaces;
using Service.Services;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Authorization; // ?


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialNetwork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IService<UserDto> service;
        private readonly IAuthService authService;
        private readonly IOwner owner;
        public UserController(IService<UserDto> service, IAuthService authService, IOwner owner)
        {
            this.service = service;
            this.authService = authService;
            this.owner = owner;
        }
        // GET: api/<UserController>
        [HttpGet]
        //[AllowAnonymous]
        public async Task<List<UserDto>> Get()
        {
            return await service.GetAll();
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public async Task<UserDto> Get(int id)
        {
            return await service.GetById(id);
        }

        // POST api/<UserController>
        [HttpPost]
        public async Task<UserDto> Post([FromForm] UserDto user)
        {
            UploadImage(user.fileImageProfile);
            return await service.Add(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] UserLogin userLogin)
        {
            var user = await authService.AuthenticateAsync(userLogin);
            if (user != null)
            {
                var token = await authService.GenerateTokenAsync(user);
                return Ok(token);
            }
            return Unauthorized("User not found");
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("GetUserByToken")]
        public async Task<UserDto> GetUserByToken()
        {
            UserDto user = GetCurrentUser();
            return user;
        }
        private int GetCurrentUserId()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userIdClaim = identity.FindFirst("userId");
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int id))
                    return id;
            }
            throw new Exception("User ID not found in token");
        }
        private UserDto GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var UserClaim = identity.Claims;
                return new UserDto()
                {
                    Name = UserClaim.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value,
                    Email = UserClaim.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value,
                    Role = (Role)Enum.Parse(typeof(Role), UserClaim.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value),
                    Password = UserClaim.FirstOrDefault(x => x.Type == ClaimTypes.PostalCode)?.Value,
                };

            }
            return null;
        }


        // PUT api/<FeedbackController>/5
        [HttpPut("{userId}")]
        [Authorize]
        public async Task<IActionResult> Put(int userId, [FromForm] UserDto user)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();
            int userIdfromClaim = int.Parse(userIdClaim);

            var isOwner = await owner.IsOwner(userId, userIdfromClaim);

            if (!isOwner && !User.IsInRole("Admin"))
                return Forbid(); // user can't update

            await service.Update(userId, user);
            return Ok();
        }


        // DELETE api/<FeedbackController>/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();
            int userId = int.Parse(userIdClaim);

            var isOwner = await owner.IsOwner(id, userId);

            if (!isOwner && !User.IsInRole("Admin"))
                return Forbid(); // user can't delete

            await service.Delete(id);
            return Ok();
        }

        private void UploadImage(IFormFile file)
        {
            //ניתוב לתמונה
            var path = Path.Combine(Environment.CurrentDirectory, "Images/", file.FileName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
            }
        }
    }
}
