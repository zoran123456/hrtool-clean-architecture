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
    public class CompanyLinksController : ControllerBase
    {
        private readonly LinkService _linkService;

        public CompanyLinksController(LinkService linkService)
        {
            _linkService = linkService;
        }

        /// <summary>
        /// Gets all company links (for dashboard display). Any authenticated user can call this.
        /// </summary>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllLinks()
        {
            var links = await _linkService.GetAllAsync();
            return Ok(links);
        }

        /// <summary>
        /// Admin: Creates a new company link.
        /// </summary>
        /// <param name="dto">The link data (title, url).</param>
        [HttpPost("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateCompanyLinkDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var link = await _linkService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = link.Id }, link);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Admin: Updates an existing company link.
        /// </summary>
        /// <param name="id">The link id.</param>
        /// <param name="dto">The updated link data.</param>
        [HttpPut("admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCompanyLinkDto dto)
        {
            var (success, error) = await _linkService.UpdateAsync(id, dto);
            if (!success) return NotFound(error);
            return NoContent();
        }

        /// <summary>
        /// Admin: Deletes a company link.
        /// </summary>
        /// <param name="id">The link id.</param>
        [HttpDelete("admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _linkService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Gets a company link by id (for admin or future use).
        /// </summary>
        /// <param name="id">The link id.</param>
        [HttpGet("admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var link = await _linkService.GetByIdAsync(id);
            if (link == null) return NotFound();
            return Ok(link);
        }
    }
}
