using System;

namespace HRTool.Application.DTOs
{
    /// <summary>
    /// DTO for creating a notification.
    /// </summary>
    public class CreateNotificationDto
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime? ExpiryDate { get; set; } // Optional
    }
}
