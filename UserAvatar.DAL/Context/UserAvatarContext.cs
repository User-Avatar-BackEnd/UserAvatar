using System;
using Microsoft.EntityFrameworkCore;
using UserAvatar.DAL.Entities;

namespace UserAvatar.DAL.Context
{
    public class UserAvatarContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Board> Boards { get; set; }
        public DbSet<Column> Columns { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<History> Histories { get; set; }
        public DbSet<Invite> Invites { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Rank> Ranks { get; set; }
        public DbSet<Task> Tasks { get; set; }
        
        public UserAvatarContext(DbContextOptions<UserAvatarContext> options)
            :base(options)
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine);
            //optionsBuilder.UseSqlite("Filename=userAvatar.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .ToTable("Users");
            modelBuilder.Entity<Column>()
                .ToTable("Columns");
            modelBuilder.Entity<Comment>()
                .ToTable("Comments");
            modelBuilder.Entity<Event>()
                .ToTable("Events");
            modelBuilder.Entity<History>()
                .ToTable("Histories");
            
            modelBuilder.Entity<Invite>()
                .ToTable("Invites");
            
            modelBuilder.Entity<Member>()
                .ToTable("Members");
            modelBuilder.Entity<Rank>()
                .ToTable("Ranks");
            modelBuilder.Entity<Task>()
                .ToTable("Tasks");
            modelBuilder.Entity<Board>()
                .ToTable("Boards");
        }
    }
}
