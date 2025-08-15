using Ikhtibar.Core.DTOs;

namespace Ikhtibar.Core.Services.Interfaces;

/// <summary>
/// Email service interface for email-specific operations
/// Handles email sending, validation, and delivery tracking
/// </summary>
public interface IEmailService
{
    Task<EmailResult> SendEmailAsync(EmailRequest request);
    Task<EmailResult> SendTemplatedEmailAsync(TemplatedEmailRequest request);
    Task<bool> ValidateEmailAsync(string email);
}
