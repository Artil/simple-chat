using ChatServer.Models.Account;
using ChatServer.Models.ChatModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatServer.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GroupsUsers>()
           .HasKey(t => new { t.GroupId, t.UserId });

            modelBuilder.Entity<GroupsUsers>()
                .HasOne(x => x.User)
                .WithMany(x => x.GroupsUsers)
                .HasForeignKey(x => x.UserId);

            modelBuilder.Entity<GroupsUsers>()
                .HasOne(x => x.ChatForGroup)
                .WithMany(x => x.GroupsUsers)
                .HasForeignKey(x => x.GroupId);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> ChatUsers;
        public DbSet<UserAccountInfo> UsersAccountInfo { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatForGroup> ChatsForGroup { get; set; }
        public DbSet<ChatForTwo> ChatsForTwo { get; set; }
        public DbSet<GroupsUsers> GroupsUsers { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageStatus> MessagesStatus { get; set; }


    }
}
