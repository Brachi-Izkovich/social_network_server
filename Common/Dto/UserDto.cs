using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Common.Dto
{
    //public enum Role
    //{
    //    New,Veteran,Admin 
    //}
    public class UserDto
    {
        //public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        //[System.Text.Json.Serialization.JsonIgnore]
        public byte[]? ArrImageProfile { get; set; }
        public IFormFile? fileImageProfile { get; set; }
        public string? ImageProfileUrl { get; set; } // זה מחזיר את שם קובץ התמונה שנשמר
        //public Role Role { get; set; }
    }
}
