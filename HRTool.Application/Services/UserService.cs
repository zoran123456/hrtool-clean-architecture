using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRTool.Application.DTOs;
using HRTool.Application.DTOs.Admin;
using HRTool.Domain.Entities;
using HRTool.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace HRTool.Application.Services
{
    /// <summary>
    /// Service for user profile management (view/update own info and admin operations).
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

        /// <summary>
        /// Gets a list of all users (admin view).
        /// </summary>
        public async Task<List<AdminUserListDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(u => new AdminUserListDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Role = u.Role,
                Department = u.Department,
                ManagerName = u.Manager != null ? $"{u.Manager.FirstName} {u.Manager.LastName}" : null,
                IsActive = u.IsOutOfOffice == false // Simulate IsActive (could add real flag if needed)
            }).ToList();
        }

        /// <summary>
        /// Gets the public people directory (all active users, basic info).
        /// </summary>
        public async Task<List<DirectoryUserDto>> GetDirectoryAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users
                .Where(u => !u.IsOutOfOffice) // Simulate IsActive
                .OrderBy(u => u.FirstName).ThenBy(u => u.LastName)
                .Select(u => new DirectoryUserDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Role = u.Role,
                    Department = u.Department,
                    City = u.Address?.City ?? string.Empty,
                    Country = u.Address?.Country ?? string.Empty
                })
                .ToList();
        }

        /// <summary>
        /// Admin creates a new user (with password, role, etc.).
        /// </summary>
        public async Task<(AdminUserListDto? User, string? Error)> CreateUserAsync(AdminCreateUserDto dto)
        {
            // Check for duplicate email
            var existing = await _userRepository.GetByEmailAsync(dto.Email.Trim());
            if (existing != null)
                return (null, "A user with this email already exists.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                Email = dto.Email.Trim(),
                DateOfBirth = dto.DateOfBirth,
                Skills = dto.Skills?.Trim() ?? string.Empty,
                Address = dto.Address,
                Department = dto.Department?.Trim() ?? string.Empty,
                CurrentProject = dto.CurrentProject?.Trim() ?? string.Empty,
                Role = dto.Role,
                ManagerId = dto.ManagerId,
                IsOutOfOffice = !dto.IsActive, // Simulate IsActive
                CreatedAt = DateTime.UtcNow
            };

            var hasher = new PasswordHasher<User>();
            user.PasswordHash = hasher.HashPassword(user, dto.Password);

            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return (new AdminUserListDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                Department = user.Department,
                ManagerName = null, // Could fetch manager if needed
                IsActive = !user.IsOutOfOffice
            }, null);
        }

        /// <summary>
        /// Admin updates any user (including role, password, IsActive, etc.).
        /// </summary>
        public async Task<(bool Success, string? Error)> UpdateUserAsync(Guid userId, AdminUpdateUserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return (false, "User not found");

            // Check for duplicate email (if changed)
            if (!string.Equals(user.Email, dto.Email.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                var existing = await _userRepository.GetByEmailAsync(dto.Email.Trim());
                if (existing != null && existing.Id != userId)
                    return (false, "A user with this email already exists.");
            }

            user.FirstName = dto.FirstName.Trim();
            user.LastName = dto.LastName.Trim();
            user.Email = dto.Email.Trim();
            user.DateOfBirth = dto.DateOfBirth;
            user.Skills = dto.Skills?.Trim() ?? string.Empty;
            user.Address = dto.Address;
            user.Department = dto.Department?.Trim() ?? string.Empty;
            user.CurrentProject = dto.CurrentProject?.Trim() ?? string.Empty;
            user.Role = dto.Role;
            user.ManagerId = dto.ManagerId;
            user.IsOutOfOffice = !dto.IsActive; // Simulate IsActive

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                var hasher = new PasswordHasher<User>();
                user.PasswordHash = hasher.HashPassword(user, dto.Password);
            }

            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();
            return (true, null);
        }

        /// <summary>
        /// Sets the out-of-office status for the specified user. Only the user or an admin can set this.
        /// </summary>
        /// <param name="userId">The user to update.</param>
        /// <param name="dto">The OOO status DTO.</param>
        /// <returns>Success and error message if any.</returns>
        public async Task<(bool Success, string? Error)> SetOutOfOfficeAsync(Guid userId, SetOutOfOfficeDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return (false, "User not found");

            var today = DateTime.UtcNow.Date;
            DateTime? until = dto.EndDate?.Date ?? today;
            if (until < today)
                return (false, "Out-of-office end date cannot be in the past.");

            // We assume any date set includes that day as out-of-office.
            user.OutOfOfficeUntil = until;
            user.IsOutOfOffice = until >= today; // Derived: out if today <= OutOfOfficeUntil

            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();
            return (true, null);
        }

        /// <summary>
        /// Gets all users who are out of office on the given date (inclusive, ignores time).
        /// </summary>
        public async Task<List<UserProfileDto>> GetOutOfOfficeUsersAsync(DateTime date)
        {
            var users = await _userRepository.GetAllAsync();
            var day = date.Date;
            return users
                .Where(u => u.OutOfOfficeUntil.HasValue && u.OutOfOfficeUntil.Value.Date >= day)
                .Select(u => new UserProfileDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    DateOfBirth = u.DateOfBirth,
                    Skills = u.Skills,
                    Address = u.Address,
                    Department = u.Department,
                    CurrentProject = u.CurrentProject,
                    ManagerName = u.Manager != null ? $"{u.Manager.FirstName} {u.Manager.LastName}" : null
                })
                .ToList();
        }
    }
}
