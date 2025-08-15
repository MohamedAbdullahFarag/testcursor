using Dapper;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for MediaType lookup operations
/// </summary>
public class MediaTypeRepository : BaseRepository<MediaType>, IMediaTypeRepository
{
    public MediaTypeRepository(IDbConnectionFactory connectionFactory, ILogger<MediaTypeRepository> logger) 
        : base(connectionFactory, logger, "MediaTypes", "MediaTypeId")
    {
    }

    /// <summary>
    /// Gets media type by name
    /// </summary>
    /// <param name="name">Media type name</param>
    /// <returns>Media type if found, null otherwise</returns>
    public async Task<MediaType?> GetByNameAsync(string name)
    {
        const string sql = "SELECT * FROM MediaTypes WHERE Name = @Name";
        
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<MediaType>(sql, new { Name = name });
    }

    /// <summary>
    /// Gets all active media types
    /// </summary>
    /// <returns>Collection of active media types</returns>
    public async Task<IEnumerable<MediaType>> GetActiveTypesAsync()
    {
        const string sql = "SELECT * FROM MediaTypes ORDER BY Name";
        
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<MediaType>(sql);
    }
}
