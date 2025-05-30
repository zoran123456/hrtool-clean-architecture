using HRTool.Domain.Entities;
using HRTool.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRTool.API.Utils
{
    /// <summary>
    /// Seeds realistic test data for all tables except the admin user.
    /// </summary>
    public static class DataSeeder
    {
        public static async Task SeedTestDataAsync(HrDbContext db)
        {
            await db.Database.EnsureCreatedAsync();

            // Only seed if there are no non-admin users
            if (await db.Users.AnyAsync(u => u.Role != Role.Admin))
                return;

            var hasher = new PasswordHasher<User>();
            var departments = new[] { "IT", "HR", "Finance", "Marketing", "Sales", "Support" };
            var projects = new[] { "Apollo", "Hermes", "Orion", "Zeus", "Athena" };
            var skills = new[] { "C#", "SQL", "Azure", "React", "Excel", "Python", "Leadership", "Communication" };
            var cities = new[] { "London", "Berlin", "Paris", "New York", "San Francisco", "Tokyo", "Sydney" };
            var countries = new[] { "UK", "Germany", "France", "USA", "Japan", "Australia" };

            var users = new List<User>();
            var random = new Random();
            for (int i = 0; i < 30; i++)
            {
                var firstName = $"Test{i + 1}";
                var lastName = $"User{i + 1}";
                var email = $"test{i + 1}@hrtool.local";
                var department = departments[random.Next(departments.Length)];
                var project = projects[random.Next(projects.Length)];
                var skillList = string.Join(", ", skills.OrderBy(_ => random.Next()).Take(random.Next(2, 5)));
                var city = cities[random.Next(cities.Length)];
                var country = countries[random.Next(countries.Length)];
                var dob = DateTime.UtcNow.AddYears(-random.Next(22, 55)).AddDays(random.Next(365));
                var createdAt = DateTime.UtcNow.AddDays(-random.Next(0, 365));
                var isOutOfOffice = random.NextDouble() < 0.1; // 10% out of office
                var outOfOfficeUntil = isOutOfOffice ? DateTime.UtcNow.AddDays(random.Next(1, 10)) : (DateTime?)null;
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Role = Role.User,
                    DateOfBirth = dob,
                    Skills = skillList,
                    Address = new HRTool.Domain.ValueObjects.Address
                    {
                        Street = $"{random.Next(1, 200)} Main St",
                        City = city,
                        Country = country
                    },
                    Department = department,
                    IsOutOfOffice = isOutOfOffice,
                    OutOfOfficeUntil = outOfOfficeUntil,
                    CurrentProject = project,
                    CreatedAt = createdAt,
                    PasswordHash = hasher.HashPassword(null!, "Test1234!")
                    // ManagerId will be set in a second step
                };
                users.Add(user);
            }

            // Step 1: Add users without ManagerId
            await db.Users.AddRangeAsync(users);
            await db.SaveChangesAsync();

            // Step 2: Assign random managers (avoid self and circular refs)
            var userIds = users.Select(u => u.Id).ToList();
            foreach (var user in users)
            {
                var possibleManagers = userIds.Where(id => id != user.Id).ToList();
                if (possibleManagers.Count > 0 && random.NextDouble() < 0.7)
                {
                    user.ManagerId = possibleManagers[random.Next(possibleManagers.Count)];
                }
            }
            db.Users.UpdateRange(users);
            await db.SaveChangesAsync();

            // Notifications
            var notifications = new List<Notification>();
            for (int i = 0; i < 15; i++)
            {
                notifications.Add(new Notification
                {
                    Id = Guid.NewGuid(),
                    Title = $"Test Notification {i + 1}",
                    Message = $"This is a test notification message number {i + 1}.",
                    ExpiryDate = DateTime.UtcNow.AddDays(random.Next(5, 60)),
                    IsActive = random.NextDouble() < 0.8
                });
            }
            await db.Notifications.AddRangeAsync(notifications);

            // Company Links
            var links = new List<CompanyLink>();
            for (int i = 0; i < 10; i++)
            {
                links.Add(new CompanyLink
                {
                    Id = Guid.NewGuid(),
                    Title = $"Resource {i + 1}",
                    Url = $"https://company.resource/{i + 1}"
                });
            }
            await db.CompanyLinks.AddRangeAsync(links);

            await db.SaveChangesAsync();
        }
    }
}
