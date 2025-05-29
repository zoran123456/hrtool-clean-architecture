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
    /// Service for managing notifications (admin and user operations).
    /// </summary>
    public class NotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(INotificationRepository notificationRepository, IUnitOfWork unitOfWork)
        {
            _notificationRepository = notificationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<NotificationDto?> GetByIdAsync(Guid id)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            if (notification == null) return null;
            return ToDto(notification);
        }

        public async Task<List<NotificationDto>> GetAllAsync(bool onlyActive = false)
        {
            var notifications = await _notificationRepository.GetAllAsync();
            var now = DateTime.UtcNow;
            var filtered = onlyActive
                ? notifications.Where(n => n.IsActive && (!n.ExpiryDate.HasValue || n.ExpiryDate.Value >= now))
                : notifications;
            return filtered
                .OrderByDescending(n => n.ExpiryDate ?? DateTime.MaxValue)
                .ThenByDescending(n => n.Id)
                .Select(ToDto)
                .ToList();
        }

        public async Task<NotificationDto> CreateAsync(CreateNotificationDto dto)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                Title = dto.Title.Trim(),
                Message = dto.Message.Trim(),
                ExpiryDate = dto.ExpiryDate,
                IsActive = true
            };
            await _notificationRepository.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();
            return ToDto(notification);
        }

        public async Task<(bool Success, string? Error)> UpdateAsync(Guid id, UpdateNotificationDto dto)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            if (notification == null) return (false, "Notification not found");
            notification.Title = dto.Title.Trim();
            notification.Message = dto.Message.Trim();
            notification.ExpiryDate = dto.ExpiryDate;
            notification.IsActive = dto.IsActive;
            _notificationRepository.Update(notification);
            await _unitOfWork.SaveChangesAsync();
            return (true, null);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            if (notification == null) return false;
            await _notificationRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private static NotificationDto ToDto(Notification n) => new NotificationDto
        {
            Id = n.Id,
            Title = n.Title,
            Message = n.Message,
            ExpiryDate = n.ExpiryDate,
            IsActive = n.IsActive
        };
    }
}
