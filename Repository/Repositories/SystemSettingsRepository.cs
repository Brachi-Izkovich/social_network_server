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
    public class SystemSettingsRepository : IRepository<SystemSettings>
    {
        private readonly IContext context;

        public SystemSettingsRepository(IContext context)
        {
            this.context = context;
        }

        public async Task<SystemSettings> Add(SystemSettings item)
        {
            await context.SystemSettings.AddAsync(item);
            await context.Save();
            return item;
        }

        public async Task Delete(int id)
        {
            var setting = await GetById(id);
            context.SystemSettings.Remove(setting);
            await context.Save();
        }

        public async Task<List<SystemSettings>> GetAll()
        {
            return await context.SystemSettings.ToListAsync();
        }

        public async Task<SystemSettings> GetById(int id)
        {
            return await context.SystemSettings.FindAsync(id);
        }

        public async Task Update(int id, SystemSettings item)
        {
            var existing = await GetById(id);
            if (existing != null)
            {
                existing.Key = item.Key;
                existing.Value = item.Value;
                await context.Save();
            }
        }
    }
}
