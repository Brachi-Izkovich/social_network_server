using AutoMapper;
using Common.Dto.User;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repository.Entities;
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
        private readonly IMapper mapper;
        public AuthService(IConfiguration config, ILoginService service, IMapper mapper)
        {
            this.config = config;
            this.service = service;
            this.mapper = mapper;
        }

        public Task<UserDto> AuthenticateAsync(UserLogin userLogin)
        {
            return AuthenticatePrivate(userLogin);
        }
        private async Task<UserDto> AuthenticatePrivate(UserLogin userLogin)
        {
            var realUser = await service.GetByUsernameAndPasswordAsync(userLogin.UserName, userLogin.Password);
            if (realUser == null) return null;
            return mapper.Map<UserDto>(realUser);

            //var allUsers = await service.GetAllUserLogin();
            //UserLogin returnUser = allUsers.FirstOrDefault(x => x.Password == userLogin.Password && x.UserName == userLogin.UserName && x.Email == userLogin.Email);

            //return mapper.Map<UserDto>(returnUser); // גם אם null, זה בסדר
        }
        public async Task<string> GenerateTokenAsync(UserLogin userLogin)
        {
            var realUser = await service.GetByUsernameAndPasswordAsync(userLogin.UserName,userLogin.Password);
            if(realUser == null)
                throw new Exception("User not found");
            return await GenerateTokenPrivate(realUser);
        }

        private async Task<string> GenerateTokenPrivate(User user)
        {
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.Name),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
                };
            var token = new JwtSecurityToken(config["Jwt:Issuer"], config["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
