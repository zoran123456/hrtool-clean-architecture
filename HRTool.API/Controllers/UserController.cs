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
