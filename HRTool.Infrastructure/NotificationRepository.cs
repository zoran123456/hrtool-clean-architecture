using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HRTool.Domain.Entities;
using HRTool.Domain.Interfaces;
using HRTool.Infrastructure.Data;

namespace HRTool.Infrastructure
{
    /// <summary>
    /// Repository implementation for Notification aggregate root.
    /// </summary>
    public class NotificationRepository : INotificationRepository
    {
        private readonly HrDbContext _context;

        public NotificationRepository(HrDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets a notification by its unique identifier.
        /// </summary>
        public async Task<Notification?> GetByIdAsync(Guid id)
        {
            return await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id);
        }

        /// <summary>
        /// Gets all notifications.
        /// </summary>
        public async Task<IEnumerable<Notification>> GetAllAsync()
        {
            return await _context.Notifications.ToListAsync();
        }

        /// <summary>
        /// Adds a new notification.
        /// </summary>
        public async Task AddAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
        }

        /// <summary>
        /// Updates an existing notification.
        /// </summary>
        public void Update(Notification notification)
        {
            _context.Notifications.Update(notification);
        }

        /// <summary>
        /// Deletes a notification by its unique identifier.
        /// </summary>
        public async Task DeleteAsync(Guid id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
            }
        }
    }
}
