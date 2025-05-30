using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HRTool.Application.DTOs;
using HRTool.Application.Services;

namespace HRTool.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly NotificationService _notificationService;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(NotificationService notificationService, ILogger<NotificationsController> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        /// <summary>
        /// Gets all active notifications (for regular users).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetActiveNotifications()
        {
            var notifications = await _notificationService.GetAllAsync(onlyActive: true);
            return Ok(notifications);
        }

        /// <summary>
        /// Admin: Creates a new notification.
        /// </summary>
        [HttpPost("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateNotificationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var notification = await _notificationService.CreateAsync(dto);
            _logger.LogInformation("Admin created notification: {Id}", notification.Id);
            return CreatedAtAction(nameof(GetById), new { id = notification.Id }, notification);
        }

        /// <summary>
        /// Admin: Updates an existing notification.
        /// </summary>
        [HttpPut("admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateNotificationDto dto)
        {
            var (success, error) = await _notificationService.UpdateAsync(id, dto);
            if (!success)
            {
                _logger.LogWarning("Failed to update notification {Id}: {Error}", id, error);
                return NotFound(error);
            }
            _logger.LogInformation("Admin updated notification: {Id}", id);
            return NoContent();
        }

        /// <summary>
        /// Admin: Deletes a notification.
        /// </summary>
        [HttpDelete("admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _notificationService.DeleteAsync(id);
            if (!deleted)
            {
                _logger.LogWarning("Failed to delete notification {Id}", id);
                return NotFound();
            }
            _logger.LogInformation("Admin deleted notification: {Id}", id);
            return NoContent();
        }

        /// <summary>
        /// Gets a notification by id (for admin or future use).
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var notification = await _notificationService.GetByIdAsync(id);
            if (notification == null)
            {
                _logger.LogWarning("Notification not found: {Id}", id);
                return NotFound();
            }
            return Ok(notification);
        }
    }
}
