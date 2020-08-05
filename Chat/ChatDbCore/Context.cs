using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using ChatDbCore.ChatModels;
using ChatDbCore.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ChatDbCore
{
    public class Context : IdentityDbContext<User>
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
        public DbSet<ForwardChatMessage> ForwardMessages { get; set; }
        public DbSet<FileDb> Files { get; set; }
    }
}
