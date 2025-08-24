using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.API.DTOs.Authentication
{
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
