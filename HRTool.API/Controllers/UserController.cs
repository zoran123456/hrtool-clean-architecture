using HRTool.Application.DTOs;
using HRTool.Application.Services;
using HRTool.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using HRTool.Application.DTOs.Admin;
using HRTool.Application.DTOs;

namespace HRTool.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(UserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Gets the current authenticated user's profile.
        /// </summary>
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userId, out var guid))
                return Unauthorized();
            var profile = await _userService.GetProfileAsync(guid);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        /// <summary>
        /// Updates the current authenticated user's profile.
        /// </summary>
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe([FromBody] UpdateUserProfileDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userId, out var guid))
                return Unauthorized();
            var (success, error) = await _userService.UpdateProfileAsync(guid, dto);
            if (!success)
            {
                _logger.LogWarning("Failed profile update for user {UserId}: {Error}", guid, error);
                return BadRequest(new { error });
            }
            _logger.LogInformation("User profile updated: {UserId}", guid);
            return NoContent();
        }

        /// <summary>
        /// Sets the out-of-office status for the current authenticated user.
        /// </summary>
        [HttpPut("me/outofoffice")]
        public async Task<IActionResult> SetOutOfOffice([FromBody] SetOutOfOfficeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userId, out var guid))
                return Unauthorized();
            var (success, error) = await _userService.SetOutOfOfficeAsync(guid, dto);
            if (!success)
            {
                _logger.LogWarning("Failed to set out-of-office for user {UserId}: {Error}", guid, error);
                return BadRequest(new { error });
            }
            _logger.LogInformation("User set out-of-office: {UserId}", guid);
            return NoContent();
        }

        /// <summary>
        /// Gets the public people directory (all active users, basic info).
        /// </summary>
        [HttpGet("/api/directory")]
        [Authorize]
        public async Task<ActionResult<List<DirectoryUserDto>>> GetDirectory()
        {
            var directory = await _userService.GetDirectoryAsync();
            return Ok(directory);
        }

        /// <summary>
        /// Gets users whose birthday is today.
        /// </summary>
        [HttpGet("birthdays/today")]
        public async Task<ActionResult<List<UserProfileDto>>> GetTodaysBirthdays()
        {
            var today = DateTime.UtcNow.Date;
            var users = await _userService.GetUsersWithBirthdayOnAsync(today);
            return Ok(users);
        }

        /// <summary>
        /// Gets users whose birthday is tomorrow.
        /// </summary>
        [HttpGet("birthdays/tomorrow")]
        public async Task<ActionResult<List<UserProfileDto>>> GetTomorrowsBirthdays()
        {
            var tomorrow = DateTime.UtcNow.Date.AddDays(1);
            var users = await _userService.GetUsersWithBirthdayOnAsync(tomorrow);
            return Ok(users);
        }

        /// <summary>
        /// Gets new employees (joined within the last X days, default 30).
        /// </summary>
        [HttpGet("new")]
        public async Task<ActionResult<List<UserProfileDto>>> GetNewEmployees([FromQuery] int days = 30)
        {
            var users = await _userService.GetRecentUsersAsync(days);
            return Ok(users);
        }

        // ------------------- ADMIN ENDPOINTS -------------------

        /// <summary>
        /// Admin: Gets all users in the system.
        /// </summary>
        /// <returns>List of all users with admin details.</returns>
        /// <response code="200">Returns the list of users</response>
        [HttpGet("admin/users")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<AdminUserListDto>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        /// <summary>
        /// Admin: Creates a new user.
        /// </summary>
        /// <param name="dto">The user data to create.</param>
        /// <returns>The created user.</returns>
        /// <response code="201">Returns the newly created user</response>
        /// <response code="409">If a user with the same email already exists</response>
        [HttpPost("admin/users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] AdminCreateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var (user, error) = await _userService.CreateUserAsync(dto);
            if (error != null)
            {
                _logger.LogWarning("Failed to create user: {Error}", error);
                return Conflict(new { error });
            }
            _logger.LogInformation("Admin created user: {UserId}", user!.Id);
            return CreatedAtAction(nameof(GetAllUsers), new { id = user!.Id }, user);
        }

        /// <summary>
        /// Admin: Updates an existing user.
        /// </summary>
        /// <param name="id">The user id.</param>
        /// <param name="dto">The updated user data.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="204">User updated successfully</response>
        /// <response code="404">User not found</response>
        /// <response code="409">Conflict if email already exists</response>
        [HttpPut("admin/users/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] AdminUpdateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var (success, error) = await _userService.UpdateUserAsync(id, dto);
            if (!success)
            {
                if (error == "User not found")
                {
                    _logger.LogWarning("User not found for update: {UserId}", id);
                    return NotFound();
                }
                if (error != null && error.Contains("exists"))
                {
                    _logger.LogWarning("Conflict updating user {UserId}: {Error}", id, error);
                    return Conflict(new { error });
                }
                _logger.LogWarning("Failed to update user {UserId}: {Error}", id, error);
                return BadRequest(new { error });
            }
            _logger.LogInformation("Admin updated user: {UserId}", id);
            return NoContent();
        }
    }
}
