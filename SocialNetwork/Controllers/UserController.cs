using Common.Dto.User;
using Microsoft.AspNetCore.Authorization; // ?
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repository.Entities;
using Service.Interfaces;
using Service.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialNetwork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IAuthService authService;
        private readonly IOwner owner;
        public UserController(IUserService userService, IAuthService authService, IOwner owner)
        {
            this.userService = userService;
            this.authService = authService;
            this.owner = owner;
        }
        // GET: api/<UserController>
        [HttpGet]
        //[AllowAnonymous]
        public async Task<List<UserDto>> Get()
        {
            return await userService.GetAll();
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public async Task<UserDto> Get(int id)
        {
            return await userService.GetById(id);
        }

        // POST api/<UserController>
        [HttpPost("Register")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Register([FromForm] UserRegisterDto user)
        {
            // Validation
            
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                return BadRequest("Email is required and cannot be empty.");
            }

            if (!user.Email.EndsWith("@gmail.com"))
            {
                return BadRequest("Email must end with '@gmail.com'.");
            }

            if (user.Email.Contains(" "))
            {
                return BadRequest("Email cannot contain spaces.");
            }

            if (string.IsNullOrWhiteSpace(user.Password))
            {
                return BadRequest("Password is required and cannot be empty.");
            }

            if (user.Password.Length < 6)
            {
                return BadRequest("Password must be at least 6 characters long.");
            }

            if (!user.Password.Any(char.IsLetter))
            {
                return BadRequest("Password must contain at least one letter.");
            }

            if (!user.Password.Any(char.IsDigit))
            {
                return BadRequest("Password must contain at least one number.");
            }

            if (user.Password.Contains(" "))
            {
                return BadRequest("Password cannot contain spaces.");
            }

            // Image

            if (user.fileImageProfile != null)
            {
                UploadImage(user.fileImageProfile);
                user.ImageProfileUrl = user.fileImageProfile.FileName;
            }
            Console.WriteLine("Length of image bytes: " + user.ArrImageProfile?.Length);

            return Ok(await userService.Add(user));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] UserLogin userLogin)
        {
            try
            {
                var token = await authService.GenerateTokenAsync(userLogin);
                return Ok(token);
            }
            catch(Exception)
            {
                return Unauthorized("user not found");
            }
        }


        //[Authorize(Roles = "Admin")]
        //[HttpGet("GetUserByToken")]
        //public async Task<UserDto> GetUserByToken()
        //{
        //    UserDto user = GetCurrentUser();
        //    return user;
        //}
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
        //private UserDto GetCurrentUser()
        //{
        //    var identity = HttpContext.User.Identity as ClaimsIdentity;
        //    if (identity != null)
        //    {
        //        var UserClaim = identity.Claims;
        //        return new UserDto()
        //        {
        //            Name = UserClaim.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value,
        //            Email = UserClaim.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value,
        //            Password = UserClaim.FirstOrDefault(x => x.Type == ClaimTypes.PostalCode)?.Value,
        //        };

        //    }
        //    return null;
        //}

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

            await userService.Update(userId, user);
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

            await userService.Delete(id);
            return Ok();
        }

        //[HttpGet("user/image/{userId}")]
        //public IActionResult GetUserImage(int userId)
        //{
        //    var user = service.GetById(userId);
        //    if (user?.ArrImageProfile == null)
        //        return NotFound();

        //    return File(user.ArrImageProfile, "image/jpeg");
        //}

        private void UploadImage(IFormFile file)
        {
            var path = Path.Combine(Environment.CurrentDirectory, "Images/", file.FileName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
            }
        }
    }
}
