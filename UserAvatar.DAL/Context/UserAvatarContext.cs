using System;
using Microsoft.EntityFrameworkCore;
using UserAvatar.DAL.Entities;

namespace UserAvatar.DAL.Context
{
    public sealed class UserAvatarContext : DbContext
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
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Invite>()
                .HasOne(invite => invite.Inviter)
                .WithMany(user => user.Inviter)
                .HasForeignKey(invite => invite.InviterId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Invite>()
                .HasOne(invite => invite.Invited)
                .WithMany(user => user.Invited)
                .HasForeignKey(invite => invite.InvitedId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}