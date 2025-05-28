using System.ComponentModel.DataAnnotations;
using HRTool.Domain.ValueObjects;

namespace HRTool.Application.DTOs
{
    /// <summary>
    /// DTO for updating user profile info (only allowed fields).
    /// </summary>
    public class UpdateUserProfileDto
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
    }
}
