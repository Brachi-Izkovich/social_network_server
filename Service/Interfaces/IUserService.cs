using Common.Dto.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetById(int id);
        Task<List<UserDto>> GetAll();
        Task<UserDto> Add(UserRegisterDto user); 
        Task Update(int id, UserDto item);
        Task Delete(int id);
        Task<List<UserForAdminDto>> GetAllForAdmin();
    }

}
