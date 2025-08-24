using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Ikhtibar.API;
using Ikhtibar.Tests.TestHelpers;
using Ikhtibar.Core.Services;
using Microsoft.AspNetCore.Authentication;
using System;
using System.IO;
using Ikhtibar.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Ikhtibar.Tests.API.TestHelpers;

// Custom factory to override registrations for integration tests.
public class IntegrationTestFactory : WebApplicationFactory<Program>
{
    // Exposed mocks and helpers so tests can seed state per-test
    private Moq.Mock<Ikhtibar.Core.Repositories.Interfaces.IUserRepository>? _userRepoMock;
    private Moq.Mock<Ikhtibar.Core.Repositories.Interfaces.IRoleRepository>? _roleRepoMock;
    private Moq.Mock<Ikhtibar.Core.Repositories.Interfaces.IUserRoleRepository>? _userRoleRepoMock;
    private Moq.Mock<AutoMapper.IMapper>? _mapperMock;

    public async Task SeedRoleAsync(Ikhtibar.Shared.Entities.Role role)
    {
        using var scope = Services.CreateScope();
        var repo = scope.ServiceProvider.GetService<Ikhtibar.Core.Repositories.Interfaces.IRoleRepository>();
        if (repo == null) throw new InvalidOperationException("IRoleRepository not registered in test host");
        await repo.AddAsync(role);
    }

    public async Task SeedUserAsync(Ikhtibar.Shared.Entities.User user)
    {
        using var scope = Services.CreateScope();
        var repo = scope.ServiceProvider.GetService<Ikhtibar.Core.Repositories.Interfaces.IUserRepository>();
        if (repo == null) throw new InvalidOperationException("IUserRepository not registered in test host");
        await repo.AddAsync(user);
    }

    public async Task AssignUserRoleAsync(int userId, int roleId)
    {
        using var scope = Services.CreateScope();
        var repo = scope.ServiceProvider.GetService<Ikhtibar.Core.Repositories.Interfaces.IUserRoleRepository>();
        if (repo == null) throw new InvalidOperationException("IUserRoleRepository not registered in test host");
        await repo.AssignRoleAsync(userId, roleId);
    }

    public Task SeedDefaultRolesAsync()
    {
        // Seed a common baseline: admin system role and a regular user role
        var admin = new Ikhtibar.Shared.Entities.Role { RoleId = 1, Code = "admin", Name = "Administrator", IsSystemRole = true, IsActive = true };
        var user = new Ikhtibar.Shared.Entities.Role { RoleId = 2, Code = "user", Name = "User", IsSystemRole = false, IsActive = true };
        return Task.WhenAll(SeedRoleAsync(admin), SeedRoleAsync(user));
    }
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
            builder.ConfigureServices(services =>
            {
            // Replace IAuditService with TestableAuditService to avoid EPPlus license usage
            var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IAuditService));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddSingleton<IAuditService>(sp => new TestableAuditService());

            // Register a test-specific database initializer that executes the test project's SQL adjustments
            services.AddHostedService<TestDatabaseInitializerHostedService>();

            // Register a simple test authentication scheme to bypass real auth in integration tests
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme = "Test";
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });

            // Ensure authorization uses authenticated user by default
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });

            // Immediately apply test DB adjustments synchronously so that schema/seed are present
            try
            {
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var factory = scope.ServiceProvider.GetService<IDbConnectionFactory>();
                if (factory != null)
                {
                    using var connection = factory.CreateConnection();
                    connection.Open();
                    var baseDir = AppContext.BaseDirectory ?? Directory.GetCurrentDirectory();
                    var scriptPath = Path.Combine(baseDir, "Data", "InitializeDatabase.sql");
                    if (File.Exists(scriptPath))
                    {
                        var sql = File.ReadAllText(scriptPath);
                        using var cmd = connection.CreateCommand();
                        cmd.CommandText = sql;
                        cmd.CommandTimeout = 60;
                        cmd.ExecuteNonQuery();

                        // Verify and log Roles table columns for debugging
                        try
                        {
                            using var colCmd = connection.CreateCommand();
                            colCmd.CommandText = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Roles' ORDER BY ORDINAL_POSITION";
                            using var reader = colCmd.ExecuteReader();
                            var cols = new System.Collections.Generic.List<string>();
                            while (reader.Read())
                            {
                                cols.Add(reader.GetString(0));
                            }
                            Console.WriteLine("[TEST-DBG] Roles table columns: " + string.Join(",", cols));
                        }
                        catch (Exception) { }
                    }
                }
            }
            catch
            {
                // Don't fail the test host startup if DB adjustments cannot be applied here; the hosted service will attempt again.
            }

            // Register in-memory mock repositories and mapper for integration tests to avoid reflection-based
            // INSERT/UPDATE against the real DB (which can fail due to production schema differences).
            try
            {
                // Remove any existing registrations for these interfaces so our mocks override them
                var toRemove = services.Where(s => s.ServiceType == typeof(Ikhtibar.Core.Repositories.Interfaces.IUserRepository)
                                                  || s.ServiceType == typeof(Ikhtibar.Core.Repositories.Interfaces.IRoleRepository)
                                                  || s.ServiceType == typeof(Ikhtibar.Core.Repositories.Interfaces.IUserRoleRepository)
                                                  || s.ServiceType == typeof(AutoMapper.IMapper)).ToList();
                foreach (var d in toRemove) services.Remove(d);

                // Create and configure mocks using centralized defaults
                var userRepoMock = new Moq.Mock<Ikhtibar.Core.Repositories.Interfaces.IUserRepository>();
                var roleRepoMock = new Moq.Mock<Ikhtibar.Core.Repositories.Interfaces.IRoleRepository>();
                var userRoleRepoMock = new Moq.Mock<Ikhtibar.Core.Repositories.Interfaces.IUserRoleRepository>();
                var mapperMock = new Moq.Mock<AutoMapper.IMapper>();

                // store references so tests can seed them
                _userRepoMock = userRepoMock;
                _roleRepoMock = roleRepoMock;
                _userRoleRepoMock = userRoleRepoMock;
                _mapperMock = mapperMock;

                // Apply default behaviors (pre-seeded admin user/roles) implemented in TestHelpers
                // Pass preSeed = true so integration tests have the expected baseline data.
                userRepoMock.ApplyDefaults(preSeed: true);
                roleRepoMock.ApplyDefaults(preSeed: true);
                userRoleRepoMock.ApplyDefaults(preSeed: true);
                mapperMock.ApplyDefaults();

                services.AddSingleton(userRepoMock.Object);
                services.AddSingleton(roleRepoMock.Object);
                services.AddSingleton(userRoleRepoMock.Object);
                services.AddSingleton(mapperMock.Object);
            }
            catch
            {
                // If mocks cannot be created for some reason, fall back to DB-backed repos.
            }
        });
    }

    // Use base CreateClient; tests can add headers per-request when needed.
    public new HttpClient CreateClient()
    {
        var c = base.CreateClient();
        // Add the header that TestAuthHandler expects so most tests are authenticated by default.
        c.DefaultRequestHeaders.Add("X-Test-Auth", "1");
        return c;
    }

    // Create a client that does not include the authentication header (for unauthenticated tests)
    public HttpClient CreateClientWithoutAuth()
    {
        return base.CreateClient();
    }
}
