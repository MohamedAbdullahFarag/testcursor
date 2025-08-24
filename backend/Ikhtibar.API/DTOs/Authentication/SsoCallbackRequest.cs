using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.API.DTOs.Authentication
{
    public class SsoCallbackRequest
    {
        [Required]
        public string Code { get; set; } = string.Empty;

        [Required]
        public string State { get; set; } = string.Empty;

        public string? Error { get; set; }
        public string? ErrorDescription { get; set; }
        public string? RedirectUri { get; set; }
    }
}
