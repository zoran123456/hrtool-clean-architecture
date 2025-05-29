using System;

namespace HRTool.Application.DTOs
{
    /// <summary>
    /// DTO for returning company link info.
    /// </summary>
    public class CompanyLinkDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}
