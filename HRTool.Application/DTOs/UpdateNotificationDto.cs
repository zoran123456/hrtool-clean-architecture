using System;
using System.ComponentModel.DataAnnotations;

namespace HRTool.Application.DTOs
{
    /// <summary>
    /// DTO for updating a notification.
    /// </summary>
    public class UpdateNotificationDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Message { get; set; } = string.Empty;

        public DateTime? ExpiryDate { get; set; } // Optional
        public bool IsActive { get; set; }
    }
}
