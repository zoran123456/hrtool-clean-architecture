using System;
using HRTool.Domain.Entities;

namespace HRTool.Application.DTOs
{
    /// <summary>
    /// DTO for the public people directory (basic user info for all employees/contractors).
    /// </summary>
    public class DirectoryUserDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public Role Role { get; set; }
        public string Department { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
}
