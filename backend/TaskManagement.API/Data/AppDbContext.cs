using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Models;

namespace TaskManagement.API.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Team> Teams { get; set; } = null!;
        public DbSet<TeamMember> TeamMembers { get; set; } = null!;
        public DbSet<TaskItem> TaskItems { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Team
            builder.Entity<Team>(entity =>
            {
                entity.HasOne(t => t.CreatedBy)
                    .WithMany()
                    .HasForeignKey(t => t.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // TeamMember - composite unique index
            builder.Entity<TeamMember>(entity =>
            {
                entity.HasIndex(tm => new { tm.UserId, tm.TeamId }).IsUnique();

                entity.HasOne(tm => tm.User)
                    .WithMany(u => u.TeamMemberships)
                    .HasForeignKey(tm => tm.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(tm => tm.Team)
                    .WithMany(t => t.Members)
                    .HasForeignKey(tm => tm.TeamId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // TaskItem
            builder.Entity<TaskItem>(entity =>
            {
                entity.HasOne(t => t.Team)
                    .WithMany(team => team.Tasks)
                    .HasForeignKey(t => t.TeamId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.AssignedTo)
                    .WithMany(u => u.AssignedTasks)
                    .HasForeignKey(t => t.AssignedToId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.CreatedBy)
                    .WithMany(u => u.CreatedTasks)
                    .HasForeignKey(t => t.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(t => t.Status)
                    .HasConversion<string>();

                entity.Property(t => t.Priority)
                    .HasConversion<string>();
            });

            // Comment
            builder.Entity<Comment>(entity =>
            {
                entity.HasOne(c => c.TaskItem)
                    .WithMany(t => t.Comments)
                    .HasForeignKey(c => c.TaskItemId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.User)
                    .WithMany(u => u.Comments)
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Notification
            builder.Entity<Notification>(entity =>
            {
                entity.HasOne(n => n.User)
                    .WithMany(u => u.Notifications)
                    .HasForeignKey(n => n.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(n => n.TaskItem)
                    .WithMany()
                    .HasForeignKey(n => n.TaskItemId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.Property(n => n.Type)
                    .HasConversion<string>();
            });
        }
    }
}
