using System;

namespace HRTool.Domain.Entities
{
    /// <summary>
    /// Represents a company link for dashboard quick access.
    /// </summary>
    public class CompanyLink
    {
        /// <summary>
        /// Unique identifier for the company link.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Title of the link.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// URL of the link.
        /// </summary>
        public string Url { get; set; } = string.Empty;
    }
}