using AutoMapper;
using Common.Dto.User;
using Repository.Entities;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Table
{
    public class UserService : IUserService, ILoginService, IOwner
    {
        private readonly IRepository<User> repository;
        private readonly IMapper mapper;
        private readonly EmailService emailService;

        public UserService(IRepository<User> repository, IMapper mapper, EmailService emailService)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.emailService = emailService;
        }

        public async Task<UserDto> Add(UserRegisterDto user)
        {
            var newUser = mapper.Map<User>(user);

            newUser.Role = Role.New;
            newUser.CountMessages = 0;
            newUser.RegistrationDate = DateTime.UtcNow;

            var addedUser = await repository.Add(newUser);

            await emailService.SendEmailAsync(
                toEmail: addedUser.Email,
                toName: addedUser.Name,
                subject: "Welcome to Social Network!",
                body: $"""
               Hello {addedUser.Name},<br/>
               Your account has been created.<br/>
               <b>Username:</b> {addedUser.Name}<br/>
               <b>Your Email:</b> {addedUser.Email}<br/>
               <b>Your Password:</b> {user.Password}
               """
            );

            return mapper.Map<UserDto>(addedUser);
        }


        public async Task Delete(int id)
        {
            await repository.Delete(id);
        }

        public async Task<List<UserDto>> GetAll()
        {
            return mapper.Map<List<UserDto>>(await repository.GetAll());
        }

        public async Task<List<UserForAdminDto>> GetAllForAdmin()
        {
            return mapper.Map<List<UserForAdminDto>>(await repository.GetAll());
        }

        public async Task<List<UserLogin>> GetAllUserLogin()
        {
            return mapper.Map<List<UserLogin>>(await repository.GetAll());
        }

        public async Task<User> GetByEmail(string email)
        {
            var all = await repository.GetAll(); // מחזיר IEnumerable<User>
            return all.FirstOrDefault(u => u.Email == email);
        }

        public async Task<UserDto> GetById(int id)
        {
            return mapper.Map<UserDto>(await repository.GetById(id));
        }

        public async Task<User?> GetByUsernameAndPasswordAsync(string username, string password)
        {
            var users = await repository.GetAll();
            var user = users.FirstOrDefault(u => u.Name == username && u.Password == password);
            return user == null ? null : mapper.Map<User>(user);
        }

        public async Task<bool> IsOwner(int userIdToChange, int userId)
        {
            var user = await repository.GetById(userIdToChange);
            return user != null && user.Id == userId;
        }

        public async Task Update(int id, UserDto item)
        {
            await repository.Update(id, mapper.Map<User>(item));
        }

    }
}
