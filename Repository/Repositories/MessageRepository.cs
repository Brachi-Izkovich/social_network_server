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
    public class MessageRepository:IRepository<Message>
    {
        private readonly IContext context;
        public MessageRepository(IContext context)
        {
            this.context = context;
        }
        public async Task<Message> Add(Message item)
        {
            await context.Messages.AddAsync(item);
            await context.Save();
            return item;
        }

        public async Task Delete(int id)
        {
            context.Messages.Remove(await GetById(id));
            await context.Save();
        }

        public async Task<List<Message>> GetAll()
        {
            return context.Messages.ToList();
        }

        public async Task<Message> GetById(int id)
        {
            return await context.Messages.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task Update(int id, Message item)
        {
            var existMessage = await GetById(id);
            if (existMessage != null)
            {
                existMessage.Content = item.Content;
                existMessage.Likes = item.Likes;
                context.Save();
            }
        }
    }
}
