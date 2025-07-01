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
        private readonly IRepository<SystemSettings> _settingsRepository;

        public AdminService(UserRepository userRepository, IRepository<SystemSettings> settingsRepository)
        {
            _userRepository = userRepository;
            _settingsRepository = settingsRepository;
        }

        private async Task<string?> GetAdminCodeAsync()
        {
            var setting = (await _settingsRepository.GetAll())
                          .FirstOrDefault(s => s.Key == "AdminCode");
            return setting?.Value;
        }

        public async Task<AdminChangeResultDto> ChangeUserRoleAsync(AdminChangeRoleDto dto)
        {
            var adminCode = await GetAdminCodeAsync();
            if (dto.AdminCode != adminCode)
                return new AdminChangeResultDto
                {
                    Success = false,
                    Message = "Invalid admin code."
                };

            var user = await _userRepository.GetById(dto.UserId);
            if (user == null)
                return new AdminChangeResultDto
                {
                    Success = false,
                    Message = "User not found."
                };

            user.Role = dto.NewRole;
            await _userRepository.Update(user.Id, user);

            return new AdminChangeResultDto
            {
                Success = true,
                Message = "Role updated successfully."
            };
        }

        public async Task<AdminChangeResultDto> ChangeAdminCodeAsync(AdminChangeCodeDto dto)
        {
            var currentCode = await GetAdminCodeAsync();

            if (dto.OldCode != currentCode)
            {
                return new AdminChangeResultDto
                {
                    Success = false,
                    Message = "Current admin code is invalid."
                };
            }

            var setting = (await _settingsRepository.GetAll())
                .FirstOrDefault(s => s.Key == "AdminCode");

            if (setting == null)
            {
                return new AdminChangeResultDto
                {
                    Success = false,
                    Message = "Admin code setting not found."
                };
            }

            setting.Value = dto.NewCode;
            await _settingsRepository.Update(setting.Id, setting);

            return new AdminChangeResultDto
            {
                Success = true,
                Message = "Admin code updated successfully."
            };
        }

    }
}
