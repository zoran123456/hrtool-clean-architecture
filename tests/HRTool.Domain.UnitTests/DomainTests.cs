using Xunit;
using FluentAssertions;
using HRTool.Domain.Entities;
using HRTool.Domain.ValueObjects;
using System;

namespace HRTool.Domain.UnitTests
{
    public class UserEntityTests
    {
        [Fact]
        public void User_CanBeCreated_WithValidProperties()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Alice",
                LastName = "Smith",
                Email = "alice@hrtool.local",
                Role = Role.User,
                DateOfBirth = new DateTime(1990, 5, 10),
                Skills = "C#,SQL",
                Address = new Address { Street = "123 Main St", City = "London", Country = "UK" },
                Department = "IT",
                IsOutOfOffice = false,
                CurrentProject = "Apollo",
                CreatedAt = DateTime.UtcNow,
                PasswordHash = "hashedpassword"
            };

            user.FirstName.Should().Be("Alice");
            user.Address.City.Should().Be("London");
            user.Role.Should().Be(Role.User);
        }

        [Fact]
        public void User_ManagerReference_CanBeSet()
        {
            var manager = new User { Id = Guid.NewGuid(), FirstName = "Manager" };
            var user = new User { Id = Guid.NewGuid(), FirstName = "Bob", Manager = manager, ManagerId = manager.Id };

            user.Manager.Should().NotBeNull();
            user.ManagerId.Should().Be(manager.Id);
        }
    }

    public class AddressValueObjectTests
    {
        [Fact]
        public void Address_Properties_AreSetCorrectly()
        {
            var address = new Address { Street = "456 Elm St", City = "Berlin", Country = "Germany" };
            address.Street.Should().Be("456 Elm St");
            address.City.Should().Be("Berlin");
            address.Country.Should().Be("Germany");
        }
    }

    public class RoleEnumTests
    {
        [Fact]
        public void Role_Enum_HasExpectedValues()
        {
            ((int)Role.Admin).Should().Be(1);
            ((int)Role.User).Should().Be(2);
        }
    }
}