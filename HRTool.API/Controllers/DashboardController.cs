using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using HRTool.Application.DTOs;
using HRTool.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HRTool.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly NotificationService _notificationService;
        private readonly LinkService _linkService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(UserService userService, NotificationService notificationService, LinkService linkService, ILogger<DashboardController> logger)
        {
            _userService = userService;
            _notificationService = notificationService;
            _linkService = linkService;
            _logger = logger;
        }

        /// <summary>
        /// Gets all dashboard data for the current user in a single call.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            // Get current user ID from JWT claims
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            // Get user profile for greeting
            var user = await _userService.GetProfileAsync(userId);
            var greeting = user != null ? $"Good morning, {user.FirstName}!" : "Good morning!";

            // Fetch dashboard data from services
            var notifications = await _notificationService.GetAllAsync(onlyActive: true);
            var outOfOfficeToday = await _userService.GetOutOfOfficeUsersAsync(DateTime.UtcNow.Date);
            var outOfOfficeTomorrow = await _userService.GetOutOfOfficeUsersAsync(DateTime.UtcNow.Date.AddDays(1));
            var birthdays = await _userService.GetUsersWithBirthdayOnAsync(DateTime.UtcNow.Date);
            var newHires = await _userService.GetRecentUsersAsync(30);
            var links = await _linkService.GetAllAsync();

            // Assemble dashboard DTO
            var dto = new DashboardDto
            {
                Greeting = greeting,
                Notifications = notifications,
                OutOfOfficeToday = outOfOfficeToday,
                OutOfOfficeTomorrow = outOfOfficeTomorrow,
                Birthdays = birthdays,
                NewHires = newHires,
                Links = links
            };

            _logger.LogInformation("Dashboard data retrieved for user: {UserId}", userId);
            return Ok(dto);
        }
    }
}
