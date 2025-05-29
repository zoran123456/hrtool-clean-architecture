using HRTool.Application.DTOs;
using HRTool.Application.Services;
using HRTool.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using HRTool.Application.DTOs.Admin;
using HRTool.Application.DTOs;

namespace HRTool.API.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
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
                return BadRequest(new { error });
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
                return BadRequest(new { error });
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
        [HttpGet("admin/users")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<AdminUserListDto>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpPost("admin/users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] AdminCreateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var (user, error) = await _userService.CreateUserAsync(dto);
            if (error != null)
                return Conflict(new { error });
            return CreatedAtAction(nameof(GetAllUsers), new { id = user!.Id }, user);
        }

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
                    return NotFound();
                if (error != null && error.Contains("exists"))
                    return Conflict(new { error });
                return BadRequest(new { error });
            }
            return NoContent();
        }
    }
}
