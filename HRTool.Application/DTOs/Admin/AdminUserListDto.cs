using System;
using HRTool.Domain.Entities;

namespace HRTool.Application.DTOs.Admin
{
    /// <summary>
    /// DTO for admin user listing (includes role, IsActive, etc.).
    /// </summary>
    public class AdminUserListDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Role Role { get; set; }
        public string Department { get; set; } = string.Empty;
        public string? ManagerName { get; set; }
        public bool IsActive { get; set; }
    }
}
