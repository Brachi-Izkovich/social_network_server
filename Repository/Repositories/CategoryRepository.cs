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
    public class CategoryRepository : IRepository<Category>
    {
        private readonly IContext context;

        public CategoryRepository(IContext context)
        {
            this.context = context;
        }
        public async Task<Category> Add(Category item)
        {
            await context.Categories.AddAsync(item);
            await context.Save();
            return item;
        }

        public async Task Delete(int id)
        {
             context.Categories.Remove(await GetById(id));
             await context.Save();
        }

        public async Task<List<Category>> GetAll()
        {
            return await context.Categories.ToListAsync();
        }

        public async Task<Category> GetById(int id)
        {
            return await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task Update(int id, Category item)
        {
            var existCategory=await GetById(id);
            if (existCategory != null)
            {
                existCategory.NameCategory = item.NameCategory;
                existCategory.Topics = item.Topics;
                await context.Save();
            }
        }
    }
}
