﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TranslationApi.Domain.Entities;

namespace TranslationApi.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<AIModel> AIModels { get; set; }
        public DbSet<ChatSession> ChatSessions { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure table names
            builder.Entity<AIModel>().ToTable("AIModel");

            // Configure relationships and constraints here
            builder.Entity<ChatSession>()
                .HasOne(cs => cs.User)
                .WithMany(u => u.ChatSessions)
                .HasForeignKey(cs => cs.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ChatSession>()
                .HasOne(cs => cs.AIModel)
                .WithMany(m => m.ChatSessions)
                .HasForeignKey(cs => cs.AIModelId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ChatMessage>()
                .HasOne(cm => cm.Session)
                .WithMany(cs => cs.Messages)
                .HasForeignKey(cm => cm.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Feedback>()
                .HasOne(f => f.Message)
                .WithMany(cm => cm.Feedbacks)
                .HasForeignKey(f => f.MessageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}