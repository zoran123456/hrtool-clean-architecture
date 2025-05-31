using Xunit;
using FluentAssertions;
using Moq;
using HRTool.Application.Services;
using HRTool.Domain.Interfaces;
using HRTool.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace HRTool.Application.UnitTests
{
    public class UserServiceTests
    {
        [Fact]
        public async Task GetProfileAsync_ReturnsProfile_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, FirstName = "John", LastName = "Doe", Email = "john@doe.com" };
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var service = new UserService(userRepoMock.Object, unitOfWorkMock.Object);

            // Act
            var result = await service.GetProfileAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result!.FirstName.Should().Be("John");
            result.LastName.Should().Be("Doe");
        }
    }
}