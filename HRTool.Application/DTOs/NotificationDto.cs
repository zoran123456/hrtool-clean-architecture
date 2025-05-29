using System;

namespace HRTool.Application.DTOs
{
    /// <summary>
    /// DTO for returning notification info.
    /// </summary>
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; }
    }
}
