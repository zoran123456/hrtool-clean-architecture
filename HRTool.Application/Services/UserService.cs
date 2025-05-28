using System;
using System.Threading.Tasks;
using HRTool.Application.DTOs;
using HRTool.Domain.Entities;
using HRTool.Domain.Interfaces;

namespace HRTool.Application.Services
{
    /// <summary>
    /// Service for user profile management (view/update own info).
    /// </summary>
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Gets the profile for the specified user.
        /// </summary>
        public async Task<UserProfileDto?> GetProfileAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;
            return new UserProfileDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                DateOfBirth = user.DateOfBirth,
                Skills = user.Skills,
                Address = user.Address,
                Department = user.Department,
                CurrentProject = user.CurrentProject,
                ManagerName = user.Manager != null ? $"{user.Manager.FirstName} {user.Manager.LastName}" : null
            };
        }

        /// <summary>
        /// Updates the profile for the specified user (only allowed fields).
        /// </summary>
        public async Task<(bool Success, string? Error)> UpdateProfileAsync(Guid userId, UpdateUserProfileDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return (false, "User not found");

            // Validation: no future DOB
            if (dto.DateOfBirth > DateTime.UtcNow)
                return (false, "Date of birth cannot be in the future.");

            // Patch allowed fields
            user.FirstName = dto.FirstName.Trim();
            user.LastName = dto.LastName.Trim();
            user.Email = dto.Email.Trim();
            user.DateOfBirth = dto.DateOfBirth;
            user.Skills = dto.Skills?.Trim() ?? string.Empty;
            user.Address = dto.Address;
            user.Department = dto.Department?.Trim() ?? string.Empty;
            user.CurrentProject = dto.CurrentProject?.Trim() ?? string.Empty;
            // Optionally update audit info (e.g. user.UpdatedAt = DateTime.UtcNow;)

            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();
            return (true, null);
        }
    }
}
