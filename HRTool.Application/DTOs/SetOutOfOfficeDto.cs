using System;
using System.ComponentModel.DataAnnotations;

namespace HRTool.Application.DTOs
{
    /// <summary>
    /// DTO for setting out-of-office status. If EndDate is null, OOO is for today only.
    /// </summary>
    public class SetOutOfOfficeDto
    {
        /// <summary>
        /// The last day the user will be out of office (inclusive). If null, OOO is for today only.
        /// </summary>
        public DateTime? EndDate { get; set; }
    }
}
