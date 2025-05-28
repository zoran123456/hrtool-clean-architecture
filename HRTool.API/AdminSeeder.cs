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
        public static async Task SeedAdminIfNoneExistsAsync(HrDbContext db, IUserRepository userRepo, IUnitOfWork unitOfWork)
        {
            await db.Database.EnsureCreatedAsync();
            var adminExists = await userRepo.GetByRoleAsync(Role.Admin) != null;
            if (!adminExists)
            {
                var password = Environment.GetEnvironmentVariable("HRTOOL_ADMIN_PASSWORD");
                if (string.IsNullOrWhiteSpace(password))
                    throw new InvalidOperationException("Admin password must be set in HRTOOL_ADMIN_PASSWORD environment variable.");

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
                admin.PasswordHash = hasher.HashPassword(admin, password);

                await userRepo.AddAsync(admin);
                await unitOfWork.SaveChangesAsync();

                // Add logging here if needed
            }
        }
    }
}
