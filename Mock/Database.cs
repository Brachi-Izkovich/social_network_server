using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mock
{
    public class Database : DbContext, IContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<SystemSettings> SystemSettings { get; set; }

        public async Task Save()
        {
            await Console.Out.WriteLineAsync("save changes.....");
            await SaveChangesAsync();
        }

        //miri = DESKTOP-FKDF8KP\SQLSERVR
        //brachi = BRACHIH-COMPUTE\SQLDATABASE
        //seminar = sql

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=DESKTOP-FKDF8KP\\SQLSERVR;database=SocialNetwork;trusted_connection=true;TrustServerCertificate=True");
        }

    }
}
