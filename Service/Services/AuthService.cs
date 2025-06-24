using Common.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration config;
        private readonly ILoginService service;
        public AuthService(IConfiguration config, ILoginService service)
        {
            this.config = config;
            this.service = service;
        }

        public Task<UserDto> AuthenticateAsync(UserLogin userLogin)
        {
            return AuthenticatePrivate(userLogin);
        }

        public async Task<string> GenerateTokenAsync(UserDto user)
        {
            var realUser = await service.GetByEmail(user.Email);
            if(realUser == null)
                throw new Exception("User not found");
            return await GenerateTokenPrivate(user,realUser.Id);
        }

        private async Task<UserDto> AuthenticatePrivate(UserLogin user)
        {
            var allUsers = await service.GetAll(); // מניח שזה מחזיר Task<IEnumerable<UserDto>>
            UserDto returnUser = allUsers.FirstOrDefault(x => x.Password == user.Password && x.Name == user.UserName && x.Email == user.Email);

            return returnUser; // גם אם null, זה בסדר
        }

        private async Task<string> GenerateTokenPrivate(UserDto user, int userId)
        {
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name,user.Name),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                //new Claim("userId", userId.ToString()) //
                };
            var token = new JwtSecurityToken(config["Jwt:Issuer"], config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
