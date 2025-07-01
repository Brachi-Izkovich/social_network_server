using Common.Dto.AdminPages;
using Microsoft.Extensions.Configuration;
using Repository.Entities;
using Repository.Interfaces;
using Repository.Repositories;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserRepository _userRepository;
        private string adminCode;

        public AdminService(UserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            adminCode = configuration["AdminSettings:AdminCode"];
        }

        public async Task<AdminChangeResultDto> ChangeUserRoleAsync(AdminChangeRoleDto dto)
        {
            if (dto.AdminCode != adminCode)
                return new AdminChangeResultDto
                {
                    Success = false,
                    Message = "קוד מנהל לא תקין."
                };

            var user = await _userRepository.GetById(dto.UserId);
            if (user == null)
                return new AdminChangeResultDto
                {
                    Success = false,
                    Message = "המשתמש לא נמצא."
                };

            user.Role = dto.NewRole;
            await _userRepository.Update(user.Id, user);

            return new AdminChangeResultDto
            {
                Success = true,
                Message = "התפקיד עודכן בהצלחה."
            };
        }

        public async Task<AdminChangeResultDto> ChangeAdminCodeAsync(AdminChangeCodeDto dto)
        {
            if (dto.AdminCode != adminCode)
            {
                return new AdminChangeResultDto
                {
                    Success = false,
                    Message = "קוד מנהל נוכחי לא תקין."
                };
            }

            adminCode = dto.NewCode;

            return new AdminChangeResultDto
            {
                Success = true,
                Message = "קוד המנהל עודכן בהצלחה."
            };
        }
    }
}
