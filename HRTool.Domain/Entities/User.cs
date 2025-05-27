using HRTool.Domain.ValueObjects;
using System;

namespace HRTool.Domain.Entities
{
    /// <summary>
    /// Represents a user in the HR system.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Unique identifier for the user.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// First name of the user.
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Last name of the user.
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Email address of the user.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Role of the user (Admin or User).
        /// </summary>
        public Role Role { get; set; }

        /// <summary>
        /// Date of birth of the user.
        /// </summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Skills of the user (comma-separated text).
        /// </summary>
        public string Skills { get; set; } = string.Empty;

        /// <summary>
        /// Address of the user.
        /// </summary>
        public Address Address { get; set; } = new Address();

        /// <summary>
        /// Department of the user.
        /// </summary>
        public string Department { get; set; } = string.Empty;

        /// <summary>
        /// Indicates if the user is out of office.
        /// </summary>
        public bool IsOutOfOffice { get; set; }

        /// <summary>
        /// Date until the user is out of office.
        /// </summary>
        public DateTime? OutOfOfficeUntil { get; set; }

        /// <summary>
        /// Reference to the user's manager (by Id).
        /// </summary>
        public Guid? ManagerId { get; set; }

        /// <summary>
        /// Reference to the user's manager
        /// </summary>
        public virtual User? Manager { get; set; }

        /// <summary>
        /// Current project of the user.
        /// </summary>
        public string CurrentProject { get; set; } = string.Empty;

        /// <summary>
        /// Date and time when the user was created (for new-hire highlighting).
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}