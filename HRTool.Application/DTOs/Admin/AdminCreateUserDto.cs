using System;
using System.ComponentModel.DataAnnotations;
using HRTool.Domain.ValueObjects;
using HRTool.Domain.Entities;

namespace HRTool.Application.DTOs.Admin
{
    /// <summary>
    /// DTO for admin to create a new user (with password, role, etc.).
    /// </summary>
    public class AdminCreateUserDto
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        [StringLength(500)]
        public string Skills { get; set; } = string.Empty;

        [Required]
        public Address Address { get; set; } = new Address();

        [StringLength(100)]
        public string Department { get; set; } = string.Empty;

        [StringLength(200)]
        public string CurrentProject { get; set; } = string.Empty;

        [Required]
        public Role Role { get; set; }

        public Guid? ManagerId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
