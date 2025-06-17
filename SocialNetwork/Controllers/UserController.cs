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
        public UserController(IService<UserDto> service, IAuthService authService)
        {
            this.service = service;
            this.authService = authService;
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
        [Authorize(Roles = "Admin")]
        public async Task<UserDto> Get(int id)
        {
            return await service.GetById(id);
        }

        // POST api/<UserController>
        [HttpPost]
        //[AllowAnonymous]
        public async Task<UserDto> Post([FromForm] UserDto user)
        {
            //try
            //{
            //    UploadImage(user.fileImageProfile);
            //}
            //catch (IOException ex)
            //{
            //    return StatusCode(500, "בעיה בהעלאת הקובץ: " + ex.Message);
            //}
            UploadImage(user.fileImageProfile);
            return await service.Add(user);
        }

        [HttpPost("login")]
        //[AllowAnonymous]
        public async Task<IActionResult> Login([FromForm] UserLogin value)
        {
            var user = await authService.AuthenticateAsync(value);
            if (user != null)
            {
                var token = await authService.GenerateTokenAsync(user);
                return Ok(token);
            }
            return Unauthorized("User not found");
        }


        [Authorize]
        [HttpGet("GetUserByToken")]
        public async Task<UserDto> GetUserByToken()
        {
            UserDto user = GetCurrentUser();

            return user;
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
                    Password = UserClaim.FirstOrDefault(x => x.Type == ClaimTypes.PostalCode)?.Value
                    // GivenName = UserClaim.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value,
                    // SurName = UserClaim.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value
                };

            }
            return null;
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task Put(int id, [FromForm] UserDto user)
        {
            UploadImage(user.fileImageProfile);
            await service.Update(id, user);
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]//הערה: באמת צריך לאפשר לכל משתמש למחוק רק את עצמו חוץ מהמנהל שיכול למחוק את כולם
        public async Task Delete(int id)
        {
            await service.Delete(id);
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
