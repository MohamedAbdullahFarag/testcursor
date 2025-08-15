using Ikhtibar.Core.DTOs;

namespace Ikhtibar.Core.Services.Interfaces;

/// <summary>
/// SMS service interface for SMS-specific operations
/// Handles SMS sending, validation, and delivery tracking
/// </summary>
public interface ISmsService
{
    Task<SmsResult> SendSmsAsync(SmsRequest request);
    Task<SmsResult> SendTemplatedSmsAsync(TemplatedSmsRequest request);
    Task<bool> ValidatePhoneNumberAsync(string phoneNumber);
}
