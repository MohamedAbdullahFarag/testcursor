using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for MediaType lookup operations
/// </summary>
public interface IMediaTypeRepository : IBaseRepository<MediaType>
{
    /// <summary>
    /// Gets media type by name
    /// </summary>
    /// <param name="name">Media type name</param>
    /// <returns>Media type if found, null otherwise</returns>
    Task<MediaType?> GetByNameAsync(string name);

    /// <summary>
    /// Gets all active media types
    /// </summary>
    /// <returns>Collection of active media types</returns>
    Task<IEnumerable<MediaType>> GetActiveTypesAsync();
}
