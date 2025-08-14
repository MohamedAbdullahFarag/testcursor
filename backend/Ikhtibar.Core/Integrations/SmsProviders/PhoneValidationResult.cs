namespace Ikhtibar.Core.Integrations.SmsProviders;
public class PhoneValidationResult
{
    /// <summary>
    /// Indicates whether the phone number is valid
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// The formatted phone number in E.164 format
    /// </summary>
    public string? FormattedPhoneNumber { get; set; }

    /// <summary>
    /// The formatted phone number (alias for FormattedPhoneNumber)
    /// </summary>
    public string? FormattedNumber { get; set; }

    /// <summary>
    /// The carrier information if available
    /// </summary>
    public string? Carrier { get; set; }

    /// <summary>
    /// Any error message if validation failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Reason for validation result
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Whether this phone number can receive SMS
    /// </summary>
    public bool CanReceiveSms { get; set; } = true;
}