using Common.Dto;
using Microsoft.Extensions.Configuration;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration configuration;
        private readonly ILoginService _userService;
        public Task<UserDto> AuthenticateAsync(UserLogin userLogin)
        {
            throw new NotImplementedException();
        }

        public Task<string> GenerateTokenAsync(UserDto user)
        {
            throw new NotImplementedException();
        }
    }
}
