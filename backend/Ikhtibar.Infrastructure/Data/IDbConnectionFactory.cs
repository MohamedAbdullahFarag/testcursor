using System.Data;

namespace Ikhtibar.Infrastructure.Data;

/// <summary>
/// Factory for creating database connections for Dapper operations
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Creates a new database connection
    /// </summary>
    /// <returns>A new database connection instance</returns>
    IDbConnection CreateConnection();

    /// <summary>
    /// Creates a new database connection asynchronously
    /// </summary>
    /// <returns>A new database connection instance</returns>
    Task<IDbConnection> CreateConnectionAsync();
}
