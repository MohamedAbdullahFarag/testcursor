namespace Ikhtibar.Shared.Models
{
    /// <summary>
    /// Refresh token statistics
    /// </summary>
    public class RefreshTokenStats
    {
        public int TotalTokens { get; set; }
        public int ActiveTokens { get; set; }
        public int ExpiredTokens { get; set; }
        public int RevokedTokens { get; set; }
        public DateTime? LastIssuedAt { get; set; }
        public DateTime? LastUsedAt { get; set; }
    }
}
