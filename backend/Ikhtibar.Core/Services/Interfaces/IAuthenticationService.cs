using Ikhtibar.Shared.DTOs;

namespace Ikhtibar.Core.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<AuthResultDto> AuthenticateAsync(LoginDto request);
        Task<AuthResultDto> RefreshTokenAsync(string refreshToken);
        Task<AuthResultDto> ProcessSsoCallbackAsync(SsoCallbackDto callbackData);
        Task<bool> RevokeTokenAsync(string refreshToken);
        Task<bool> ValidateTokenAsync(string accessToken);
    }
}
