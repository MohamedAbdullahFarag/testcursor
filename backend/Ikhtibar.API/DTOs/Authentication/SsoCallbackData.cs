namespace Ikhtibar.API.DTOs.Authentication
{
    public class SsoCallbackData
    {
        public string Code { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string? Error { get; set; }
        public string? ErrorDescription { get; set; }
        public string? RedirectUri { get; set; }
    }
}
