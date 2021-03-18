using System;
using Microsoft.EntityFrameworkCore;
using UserAvatar.DAL.Entities;

namespace UserAvatar.DAL.Context
{
    public class UserAvatarContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public UserAvatarContext(DbContextOptions<UserAvatarContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           // optionsBuilder.LogTo(Console.WriteLine);
            //optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=qwerty");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .ToTable("Users");
        }
    }
}
