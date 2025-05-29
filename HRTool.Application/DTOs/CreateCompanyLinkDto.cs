using System.ComponentModel.DataAnnotations;

namespace HRTool.Application.DTOs
{
    /// <summary>
    /// DTO for creating a company link.
    /// </summary>
    public class CreateCompanyLinkDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Url { get; set; } = string.Empty;
    }
}
