using Ikhtibar.Shared.DTOs;

namespace Ikhtibar.API.DTOs.Authentication
{
    public class AuthResult
    {
        public UserDto User { get; set; } = null!;
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string TokenType { get; set; } = "Bearer";
    }
}
