using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRTool.Application.DTOs;
using HRTool.Domain.Entities;
using HRTool.Domain.Interfaces;

namespace HRTool.Application.Services
{
    /// <summary>
    /// Service for managing company links (admin and user operations).
    /// </summary>
    public class LinkService
    {
        private readonly ICompanyLinkRepository _linkRepository;
        private readonly IUnitOfWork _unitOfWork;

        public LinkService(ICompanyLinkRepository linkRepository, IUnitOfWork unitOfWork)
        {
            _linkRepository = linkRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CompanyLinkDto?> GetByIdAsync(Guid id)
        {
            var link = await _linkRepository.GetByIdAsync(id);
            return link == null ? null : ToDto(link);
        }

        public async Task<List<CompanyLinkDto>> GetAllAsync()
        {
            var links = await _linkRepository.GetAllAsync();
            return links.Select(ToDto).ToList();
        }

        public async Task<CompanyLinkDto> CreateAsync(CreateCompanyLinkDto dto)
        {
            if (!IsValidUrl(dto.Url))
                throw new ArgumentException("URL must start with http or https.");
            var link = new CompanyLink
            {
                Id = Guid.NewGuid(),
                Title = dto.Title.Trim(),
                Url = dto.Url.Trim()
            };
            await _linkRepository.AddAsync(link);
            await _unitOfWork.SaveChangesAsync();
            return ToDto(link);
        }

        public async Task<(bool Success, string? Error)> UpdateAsync(Guid id, UpdateCompanyLinkDto dto)
        {
            var link = await _linkRepository.GetByIdAsync(id);
            if (link == null) return (false, "Link not found");
            if (!IsValidUrl(dto.Url))
                return (false, "URL must start with http or https.");
            link.Title = dto.Title.Trim();
            link.Url = dto.Url.Trim();
            _linkRepository.Update(link);
            await _unitOfWork.SaveChangesAsync();
            return (true, null);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var link = await _linkRepository.GetByIdAsync(id);
            if (link == null) return false;
            await _linkRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private static bool IsValidUrl(string url)
        {
            return url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                   url.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
        }

        private static CompanyLinkDto ToDto(CompanyLink link) => new CompanyLinkDto
        {
            Id = link.Id,
            Title = link.Title,
            Url = link.Url
        };
    }
}
