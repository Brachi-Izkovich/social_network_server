using Common.Dto.AdminPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IAdminService
    {
        Task<AdminChangeResultDto> ChangeUserRoleAsync(AdminChangeRoleDto dto);
        Task<AdminChangeResultDto> ChangeAdminCodeAsync(AdminChangeCodeDto dto);
    }
}
