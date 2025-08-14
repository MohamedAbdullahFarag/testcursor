using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Ikhtibar.Infrastructure.Data;

/// <summary>
/// Implementation of database connection factory for SQL Server using Dapper
/// </summary>
public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    /// <summary>
    /// Initializes a new instance of the DbConnectionFactory
    /// </summary>
    /// <param name="configuration">Configuration containing connection strings</param>
    public DbConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("IkhtibarDatabase")
            ?? throw new InvalidOperationException("Connection string 'IkhtibarDatabase' not found");
    }

    /// <summary>
    /// Creates a new SQL Server database connection
    /// </summary>
    /// <returns>A new SqlConnection instance</returns>
    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }

    /// <summary>
    /// Creates a new SQL Server database connection asynchronously
    /// </summary>
    /// <returns>A new SqlConnection instance</returns>
    public async Task<IDbConnection> CreateConnectionAsync()
    {
        var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }
}
