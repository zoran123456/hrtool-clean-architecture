namespace HRTool.Application.DTOs
{
    /// <summary>
    /// DTO for returning user profile info (excluding sensitive fields).
    /// </summary>
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Skills { get; set; } = string.Empty;
        public HRTool.Domain.ValueObjects.Address Address { get; set; } = new();
        public string Department { get; set; } = string.Empty;
        public string CurrentProject { get; set; } = string.Empty;
        public string? ManagerName { get; set; }
    }
}
