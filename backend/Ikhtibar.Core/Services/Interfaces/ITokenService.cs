using System.Security.Claims;
using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Core.Services.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateJwtAsync(User user);
        Task<string> GenerateRefreshTokenAsync();
        Task<ClaimsPrincipal> ValidateJwtAsync(string token);
        Task<bool> IsTokenValidAsync(string token);
        Task<ClaimsPrincipal> GetPrincipalFromExpiredTokenAsync(string token);
    }
}
