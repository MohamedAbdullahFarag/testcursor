using System.IO;
using System.Linq;
using Xunit;

namespace Ikhtibar.Tests.Data
{
    public class InitializeDatabaseSchemaTests
    {
        private static readonly string SqlPath = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Ikhtibar.Infrastructure", "Data", "InitializeDatabase.sql")
        );

        [Fact]
        public void RefreshTokens_HasRequiredColumns_ForLoginFlow()
        {
            Assert.True(File.Exists(SqlPath), $"InitializeDatabase.sql not found at {SqlPath}");
            var sql = File.ReadAllText(SqlPath);

            // Columns required by repositories/entities used during login/refresh
            string[] requiredColumns = new[]
            {
                "TokenHash",
                "UserId",
                "IssuedAt",
                "ExpiresAt",
                // Soft-delete fields referenced by repository operations
                "IsDeleted",
                "DeletedAt",
                "DeletedBy",
                // Reason column aligned to entity property
                "RevocationReason",
                // Metadata captured at issuance
                "ClientIpAddress",
                "UserAgent"
            };

            foreach (var col in requiredColumns)
            {
                Assert.Contains(col, sql);
            }
        }

        [Fact]
        public void Users_HasRequiredColumns_ForLoginFlow()
        {
            Assert.True(File.Exists(SqlPath), $"InitializeDatabase.sql not found at {SqlPath}");
            var sql = File.ReadAllText(SqlPath);

            // Ensure CREATE TABLE Users block contains the required login columns
            string[] requiredColumns = new[]
            {
                "Username",
                "Email",
                "PasswordHash",
                "IsActive",
                "EmailVerified",
                "PhoneVerified",
                "CreatedAt",
                "ModifiedAt",
                "IsDeleted"
            };

            foreach (var col in requiredColumns)
            {
                Assert.Contains(col, sql);
            }
        }
    }
}
