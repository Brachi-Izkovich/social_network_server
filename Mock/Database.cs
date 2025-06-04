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
        //home = DESKTOP-FKDF8KP\SQLSERVR
        //seminar = sql
        public DbSet<User> Users { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Message> Messages { get; set; }

        public async Task Save()
        {
            await Console.Out.WriteLineAsync("save changes.....");
            await SaveChangesAsync();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=DESKTOP-FKDF8KP\\SQLSERVR;database=SocialNetwork;trusted_connection=true;TrustServerCertificate=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Topic - כאשר מוחקים את ה-User -> להגדיר ל-NULL (SetNull)
            modelBuilder.Entity<Topic>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Topic - כאשר מוחקים את ה-Category -> להגדיר ל-NULL (SetNull)
            modelBuilder.Entity<Topic>()
                .HasOne(t => t.Category)
                .WithMany()
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            // Message - כאשר מוחקים את ה-User -> להגדיר ל-NULL (SetNull)
            modelBuilder.Entity<Message>()
                .HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Message - כאשר מוחקים את ה-Topic -> למחוק את כל ה-Messages (Cascade)
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Topic)
                .WithMany(t => t.ListMessages)
                .HasForeignKey(m => m.TopicId)
                .OnDelete(DeleteBehavior.Cascade);

            // Feedback - כאשר מוחקים את ה-User -> להגדיר ל-NULL (SetNull)
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            //// Feedback - כאשר מוחקים את ה-Message -> למחוק את כל ה-Feedbacks (Cascade)
            //modelBuilder.Entity<Feedback>()
            //    .HasOne(f => f.Message)
            //    .WithMany(m => m.Feedbacks)
            //    .HasForeignKey(f => f.MessageId)
            //    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Topic>()
                .Navigation(t => t.ListMessages)
                .UsePropertyAccessMode(PropertyAccessMode.Property);
                

            //modelBuilder.Entity<Message>()
            //    .Navigation(m => m.Feedbacks)
            //    .UsePropertyAccessMode(PropertyAccessMode.Property)
            //    .IsRequired(false);
        }


    }
}
