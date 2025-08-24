namespace Ikhtibar.Shared.DTOs.Authentication
{
    public class OidcUserInfo
    {
        public string Sub { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? GivenName { get; set; }
        public string? FamilyName { get; set; }
        public string? Email { get; set; }
        public bool EmailVerified { get; set; }
        public string? Picture { get; set; }
        public string? Locale { get; set; }
    }
}
