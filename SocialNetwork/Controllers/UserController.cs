using Microsoft.AspNetCore.Mvc;
using Common.Dto;
using Service.Interfaces;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text; // ?


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialNetwork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IService<UserDto> service;
        //private readonly IConfiguration config;
        public UserController(IService<UserDto> service, IConfiguration config)
        {
            this.service = service;
            //this.config = config;
        }
        // GET: api/<UserController>
        [HttpGet]
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

        //[HttpPost("{id}")]
        //public string Login([FromForm] UserLogin user)
        //{
        //    var testUser = Authenticate(user);
        //    var token = Generate(testUser);
        //    return token;
        //}
        //private async Task<UserDto> Authenticate(UserLogin user)
        //{
        //    UserDto returnUser = await service.GetAll().FirstOrDefault(x => x.Password == user.Password && x.Name == user.UserName);
        //    if (returnUser != null)
        //        return returnUser;
        //    return null;
        //}
        //private string Generate(UserDto user)
        //{
        //    var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
        //    var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);
        //    var claims = new[] {
        //        new Claim(ClaimTypes.NameIdentifier,user.Name),
        //        new Claim(ClaimTypes.Email,user.Email),
        //        //new Claim(ClaimTypes.Name,user.Name),
        //        ///new Claim(ClaimTypes.Role,user.Role),
        //        //new Claim(ClaimTypes.GivenName,user.Name)
        //        };
        //    var token = new JwtSecurityToken(config["Jwt:Issuer"], config["Jwt:Audience"],
        //        claims,
        //        expires: DateTime.Now.AddMinutes(15),
        //        signingCredentials: credentials);
        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public async Task Put(int id, [FromForm] UserDto user)
        {
            UploadImage(user.fileImageProfile);
            await service.Update(id, user);
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
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
