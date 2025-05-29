using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HRTool.Application.DTOs;
using HRTool.Application.Services;

namespace HRTool.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly NotificationService _notificationService;

        public NotificationsController(NotificationService notificationService)
        {
            _notificationService = notificationService;
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
            if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Message))
                return BadRequest("Title and Message are required.");
            var notification = await _notificationService.CreateAsync(dto);
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
            if (!success) return NotFound(error);
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
            if (!deleted) return NotFound();
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
            if (notification == null) return NotFound();
            return Ok(notification);
        }
    }
}
