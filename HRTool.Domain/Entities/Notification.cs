using System;

namespace HRTool.Domain.Entities
{
    /// <summary>
    /// Represents a notification in the HR system.
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// Unique identifier for the notification.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Title of the notification.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Message content of the notification.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Expiry date of the notification (optional).
        /// </summary>
        public DateTime? ExpiryDate { get; set; }

        /// <summary>
        /// Indicates if the notification is active.
        /// </summary>
        public bool IsActive { get; set; }
    }
}