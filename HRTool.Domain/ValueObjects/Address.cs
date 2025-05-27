namespace HRTool.Domain.ValueObjects
{
    /// <summary>
    /// Represents a structured address value object.
    /// </summary>
    public class Address
    {
        /// <summary>
        /// Street address.
        /// </summary>
        public string Street { get; set; } = string.Empty;

        /// <summary>
        /// City name.
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Country name.
        /// </summary>
        public string Country { get; set; } = string.Empty;
    }
}