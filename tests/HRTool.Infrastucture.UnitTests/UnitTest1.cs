using Xunit;
using FluentAssertions;
using HRTool.Infrastructure;
using HRTool.Infrastructure.Data;
using HRTool.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace HRTool.Infrastucture.UnitTests
{
    public class UserRepositoryTests
    {
        private HrDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<HrDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new HrDbContext(options);
        }

        [Fact]
        public async Task AddAsync_And_GetByIdAsync_WorksCorrectly()
        {
            using var db = CreateInMemoryDbContext();
            var repo = new UserRepository(db);
            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Test",
                LastName = "User",
                Email = "test@hrtool.local",
                Role = Role.User,
                DateOfBirth = new DateTime(1990, 1, 1),
                Department = "IT",
                PasswordHash = "hash",
                Address = new HRTool.Domain.ValueObjects.Address { Street = "1 St", City = "City", Country = "Country" },
                CreatedAt = DateTime.UtcNow
            };
            await repo.AddAsync(user);
            await db.SaveChangesAsync();

            var fetched = await repo.GetByIdAsync(user.Id);
            fetched.Should().NotBeNull();
            fetched!.Email.Should().Be("test@hrtool.local");
        }

        [Fact]
        public async Task GetByEmailAsync_ReturnsCorrectUser()
        {
            using var db = CreateInMemoryDbContext();
            var repo = new UserRepository(db);
            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane@hrtool.local",
                Role = Role.Admin,
                DateOfBirth = new DateTime(1985, 2, 2),
                Department = "HR",
                PasswordHash = "hash2",
                Address = new HRTool.Domain.ValueObjects.Address { Street = "2 St", City = "Town", Country = "Country" },
                CreatedAt = DateTime.UtcNow
            };
            await repo.AddAsync(user);
            await db.SaveChangesAsync();

            var fetched = await repo.GetByEmailAsync("jane@hrtool.local");
            fetched.Should().NotBeNull();
            fetched!.FirstName.Should().Be("Jane");
        }

        [Fact]
        public async Task DeleteAsync_RemovesUser()
        {
            using var db = CreateInMemoryDbContext();
            var repo = new UserRepository(db);
            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Delete",
                LastName = "Me",
                Email = "delete@hrtool.local",
                Role = Role.User,
                DateOfBirth = new DateTime(1995, 3, 3),
                Department = "Finance",
                PasswordHash = "hash3",
                Address = new HRTool.Domain.ValueObjects.Address { Street = "3 St", City = "Village", Country = "Country" },
                CreatedAt = DateTime.UtcNow
            };
            await repo.AddAsync(user);
            await db.SaveChangesAsync();

            await repo.DeleteAsync(user.Id);
            await db.SaveChangesAsync();

            var fetched = await repo.GetByIdAsync(user.Id);
            fetched.Should().BeNull();
        }
    }
}