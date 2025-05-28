using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HRTool.Domain.Entities;

namespace HRTool.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Notification aggregate root.
    /// </summary>
    public interface INotificationRepository
    {
        /// <summary>
        /// Gets a notification by its unique identifier.
        /// </summary>
        Task<Notification?> GetByIdAsync(Guid id);

        /// <summary>
        /// Gets all notifications.
        /// </summary>
        Task<IEnumerable<Notification>> GetAllAsync();

        /// <summary>
        /// Adds a new notification.
        /// </summary>
        Task AddAsync(Notification notification);

        /// <summary>
        /// Updates an existing notification.
        /// </summary>
        void Update(Notification notification);

        /// <summary>
        /// Deletes a notification by its unique identifier.
        /// </summary>
        Task DeleteAsync(Guid id);
    }
}