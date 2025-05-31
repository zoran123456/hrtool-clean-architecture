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
    public class LinkServiceTests
    {
        [Fact]
        public async Task GetByIdAsync_ReturnsLink_WhenExists()
        {
            var id = Guid.NewGuid();
            var link = new CompanyLink { Id = id, Title = "HR", Url = "https://hr.com" };
            var repoMock = new Mock<ICompanyLinkRepository>();
            repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(link);
            var uowMock = new Mock<IUnitOfWork>();
            var service = new LinkService(repoMock.Object, uowMock.Object);

            var result = await service.GetByIdAsync(id);

            result.Should().NotBeNull();
            result!.Title.Should().Be("HR");
        }

        [Fact]
        public async Task CreateAsync_Throws_WhenUrlInvalid()
        {
            var repoMock = new Mock<ICompanyLinkRepository>();
            var uowMock = new Mock<IUnitOfWork>();
            var service = new LinkService(repoMock.Object, uowMock.Object);
            var dto = new CreateCompanyLinkDto { Title = "Bad", Url = "ftp://bad" };

            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(dto));
        }
    }
}
