using Xunit;
using FluentAssertions;
using Moq;
using HRTool.Application.Services;
using HRTool.Domain.Interfaces;
using HRTool.Domain.Entities;
using HRTool.Application.DTOs;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HRTool.Application.UnitTests
{
    public class NotificationServiceTests
    {
        [Fact]
        public async Task GetByIdAsync_ReturnsNotification_WhenExists()
        {
            var id = Guid.NewGuid();
            var notification = new Notification { Id = id, Title = "Test", Message = "Msg", IsActive = true };
            var repoMock = new Mock<INotificationRepository>();
            repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(notification);
            var uowMock = new Mock<IUnitOfWork>();
            var service = new NotificationService(repoMock.Object, uowMock.Object);

            var result = await service.GetByIdAsync(id);

            result.Should().NotBeNull();
            result!.Title.Should().Be("Test");
        }

        [Fact]
        public async Task CreateAsync_AddsNotification()
        {
            var repoMock = new Mock<INotificationRepository>();
            var uowMock = new Mock<IUnitOfWork>();
            var service = new NotificationService(repoMock.Object, uowMock.Object);
            var dto = new CreateNotificationDto { Title = "New", Message = "Body" };

            var result = await service.CreateAsync(dto);

            result.Title.Should().Be("New");
            repoMock.Verify(r => r.AddAsync(It.IsAny<Notification>()), Times.Once);
            uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}
