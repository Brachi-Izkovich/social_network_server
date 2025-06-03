using Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IAuthService
    {
        Task<string> GenerateTokenAsync(UserDto user);
        Task<UserDto> AuthenticateAsync(UserLogin userLogin);
    }
}
