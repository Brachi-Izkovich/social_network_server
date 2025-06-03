using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class UserRepository : IRepository<User>
    {
        private readonly IContext context;
        public UserRepository(IContext context)
        {
            this.context = context;
        }
        public async Task<User> Add(User item)
        {
            await context.Users.AddAsync(item);
            await context.Save();
            return item;
        }

        public async Task Delete(int id)
        {
            context.Users.Remove(await GetById(id));
            await context.Save();
        }

        public async Task<List<User>> GetAll()
        {
            return await context.Users.ToListAsync();
        }

        public async Task<User> GetById(int id)
        {
            return await context.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task Update(int id, User item)
        {
            var existUser = await GetById(id);   
            if(existUser != null)
            {
                existUser.Name = item.Name;
                existUser.Password = item.Password;
                existUser.Role = item.Role;
                existUser.Email = item.Email;
                existUser.ImageProfileUrl = item.ImageProfileUrl;
                existUser.CountMessages = item.CountMessages;
                await context.Save();
            }
        }
    }
}
