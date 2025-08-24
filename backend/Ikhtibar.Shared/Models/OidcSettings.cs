namespace Ikhtibar.Shared.Models
{
    public class OidcSettings
    {
        public string Authority { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string[] Scopes { get; set; } = { "openid", "profile", "email" };
        public string ResponseType { get; set; } = "code";
        public bool UsePkce { get; set; } = true;
    }
}
