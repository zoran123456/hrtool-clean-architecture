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
        [DataType(DataType.Date)]
        [FutureOrToday]
        public DateTime? EndDate { get; set; }
    }

    /// <summary>
    /// Validation attribute to ensure date is today or in the future (if provided).
    /// </summary>
    public class FutureOrTodayAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;
            if (value is DateTime date)
            {
                if (date.Date < DateTime.UtcNow.Date)
                    return new ValidationResult("EndDate cannot be in the past.");
            }
            return ValidationResult.Success;
        }
    }
}
