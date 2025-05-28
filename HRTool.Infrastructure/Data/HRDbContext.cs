using Microsoft.EntityFrameworkCore;
using HRTool.Domain.Entities;
using HRTool.Domain.ValueObjects;

namespace HRTool.Infrastructure.Data
{
    public class HrDbContext : DbContext
    {
        public HrDbContext(DbContextOptions<HrDbContext> opts)
            : base(opts) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<CompanyLink> CompanyLinks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User entity configuration
            modelBuilder.Entity<User>(b =>
            {
                b.HasIndex(u => u.Email).IsUnique();
                b.Property(u => u.Email).HasMaxLength(200).IsRequired();
                b.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
                b.Property(u => u.LastName).HasMaxLength(100).IsRequired();
                b.Property(u => u.Department).HasMaxLength(100);
                b.Property(u => u.Skills).HasMaxLength(500);
                b.Property(u => u.CurrentProject).HasMaxLength(200);
                b.Property(u => u.PasswordHash).IsRequired();
                b.HasOne(u => u.Manager)
                 .WithMany()
                 .HasForeignKey(u => u.ManagerId)
                 .OnDelete(DeleteBehavior.Restrict);
                // Address as owned type
                b.OwnsOne(u => u.Address, a =>
                {
                    a.Property(ad => ad.Street).HasMaxLength(200).IsRequired();
                    a.Property(ad => ad.City).HasMaxLength(100).IsRequired();
                    a.Property(ad => ad.Country).HasMaxLength(100).IsRequired();
                });
            });

            // Notification entity configuration
            modelBuilder.Entity<Notification>(b =>
            {
                b.Property(n => n.Title).HasMaxLength(200).IsRequired();
                b.Property(n => n.Message).HasMaxLength(1000).IsRequired();
            });

            // CompanyLink entity configuration
            modelBuilder.Entity<CompanyLink>(b =>
            {
                b.Property(c => c.Title).HasMaxLength(200).IsRequired();
                b.Property(c => c.Url).HasMaxLength(500).IsRequired();
            });
        }
    }
}