using System;
using Microsoft.EntityFrameworkCore;
using UserAvatar.DAL.Entities;

namespace UserAvatar.DAL.Context
{
    public class UserAvatarContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public UserAvatarContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine);
            optionsBuilder.UseSqlite("Filename=userAvatar.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .ToTable("Users");
        }
    }
}
