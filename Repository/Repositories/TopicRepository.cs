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
    public class TopicRepository : IRepository<Topic>
    {
        private readonly IContext context;
        public TopicRepository(IContext context)
        {
            this.context = context;
        }
        public async Task<Topic> Add(Topic item)
        {
            await context.Topics.AddAsync(item);
            await context.Save();
            return item;
        }

        public async Task Delete(int id)
        {
            context.Topics.Remove(await GetById(id));
            await context.Save();
        }

        public async Task<List<Topic>> GetAll()
        {
           return context.Topics.ToList();
        }

        public async Task<Topic> GetById(int id)
        {
            return await context.Topics.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task Update(int id, Topic item)
        {
            var existTopic = await GetById(id);
            if(existTopic != null)
            {
                existTopic.Title = item.Title;
                existTopic.ListMessages = item.ListMessages;
                existTopic.CategoryId = item.CategoryId;
                context.Save();
            }
        }
    }
}
