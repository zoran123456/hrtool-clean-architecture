using System.Collections.Generic;

namespace HRTool.Application.DTOs
{
    /// <summary>
    /// Aggregated dashboard data for the home page.
    /// </summary>
    public class DashboardDto
    {
        public string Greeting { get; set; } = string.Empty;
        public List<NotificationDto> Notifications { get; set; } = new();
        public List<UserProfileDto> OutOfOfficeToday { get; set; } = new();
        public List<UserProfileDto> OutOfOfficeTomorrow { get; set; } = new();
        public List<UserProfileDto> Birthdays { get; set; } = new();
        public List<UserProfileDto> NewHires { get; set; } = new();
        public List<CompanyLinkDto> Links { get; set; } = new();
    }
}
