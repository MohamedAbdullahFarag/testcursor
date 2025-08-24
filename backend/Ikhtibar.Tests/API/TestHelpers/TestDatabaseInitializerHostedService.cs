using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using Ikhtibar.Infrastructure.Data;

namespace Ikhtibar.Tests.API.TestHelpers;

public class TestDatabaseInitializerHostedService : IHostedService
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<TestDatabaseInitializerHostedService> _logger;

    public TestDatabaseInitializerHostedService(IServiceProvider provider, ILogger<TestDatabaseInitializerHostedService> logger)
    {
        _provider = provider;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _provider.CreateScope();
            var factory = scope.ServiceProvider.GetService<IDbConnectionFactory>();
            if (factory == null)
            {
                _logger.LogWarning("IDbConnectionFactory not registered; skipping test DB adjustments");
                return Task.CompletedTask;
            }

            using var connection = factory.CreateConnection();
            connection.Open();

            // Try to execute the test project's InitializeDatabase.sql from the test output folder
            var baseDir = AppContext.BaseDirectory ?? Directory.GetCurrentDirectory();
            var scriptPath = Path.Combine(baseDir, "Data", "InitializeDatabase.sql");

            if (File.Exists(scriptPath))
            {
                var sql = File.ReadAllText(scriptPath);
                using var cmd = connection.CreateCommand();
                cmd.CommandText = sql;
                cmd.CommandTimeout = 60;
                cmd.ExecuteNonQuery();
                _logger.LogInformation("Test DB adjustments applied from {scriptPath}", scriptPath);
            }
            else
            {
                // Fallback: ensure minimal IsActive column exists
                var checkColumnSql = @"IF OBJECT_ID('dbo.Roles') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.Roles','IsActive') IS NULL
    BEGIN
        ALTER TABLE dbo.Roles ADD IsActive BIT NOT NULL CONSTRAINT DF_Roles_IsActive DEFAULT(1);
    END
END";
                using var cmd = connection.CreateCommand();
                cmd.CommandText = checkColumnSql;
                cmd.CommandTimeout = 30;
                cmd.ExecuteNonQuery();
                _logger.LogInformation("Fallback Test DB adjustments applied");
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to apply test DB adjustments; continuing");
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
