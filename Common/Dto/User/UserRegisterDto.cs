using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto.User
{
    public class UserRegisterDto
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public byte[]? ArrImageProfile { get; set; }
        public IFormFile? fileImageProfile { get; set; }
        public string? ImageProfileUrl { get; set; }
    }
}
