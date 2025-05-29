using System;

namespace HRTool.Application.DTOs
{
    /// <summary>
    /// DTO for updating a notification.
    /// </summary>
    public class UpdateNotificationDto
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime? ExpiryDate { get; set; } // Optional
        public bool IsActive { get; set; }
    }
}
