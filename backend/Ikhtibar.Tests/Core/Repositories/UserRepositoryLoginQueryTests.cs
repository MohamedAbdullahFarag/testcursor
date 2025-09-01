using System.Threading.Tasks;
using Xunit;
using Ikhtibar.Infrastructure.Repositories;

namespace Ikhtibar.Tests.Core.Repositories
{
    public class UserRepositoryLoginQueryTests
    {
        [Fact]
        public void GetByEmailAsync_SelectsExpectedColumns()
        {
            // Reflection inspect the const SQL in source to ensure critical columns are present
            var type = typeof(UserRepository);
            var method = type.GetMethod("GetByEmailAsync");
            Assert.NotNull(method);

            var repoPath = System.IO.Path.GetFullPath(
                System.IO.Path.Combine(System.AppContext.BaseDirectory, "..", "..", "..", "..", "Ikhtibar.Infrastructure", "Repositories", "UserRepository.cs")
            );
            Assert.True(System.IO.File.Exists(repoPath), $"UserRepository.cs not found at {repoPath}");
            var source = System.IO.File.ReadAllText(repoPath);

            // Basic asserts to catch regressions where needed columns are dropped
            Assert.Contains("PasswordHash", source);
            Assert.Contains("EmailVerified", source);
            Assert.Contains("IsActive", source);
            Assert.Contains("UserRoles", source);
            Assert.Contains("Roles r", source);
        }
    }
}
