using HRTool.Domain.Entities;
using HRTool.Domain.Interfaces;
using HRTool.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;

namespace HRTool.API
{
    public static class AdminSeeder
    {
        public static void SeedAdminIfNoneExists(HrDbContext db, IUserRepository userRepo)
        {
            db.Database.EnsureCreated();
            var adminExists = db.Users.Any(u => u.Role == Role.Admin);
            if (!adminExists)
            {
                var hasher = new PasswordHasher<User>();
                var admin = new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Admin",
                    LastName = "User",
                    Email = "admin@hrtool.local",
                    Role = Role.Admin,
                    DateOfBirth = DateTime.UtcNow.AddYears(-30),
                    Skills = "",
                    Address = new HRTool.Domain.ValueObjects.Address { Street = "", City = "", Country = "" },
                    Department = "IT",
                    IsOutOfOffice = false,
                    CreatedAt = DateTime.UtcNow,
                    CurrentProject = "",
                };
                admin.PasswordHash = hasher.HashPassword(admin, "Admin@123");
                db.Users.Add(admin);
                db.SaveChanges();
            }
        }
    }
}
