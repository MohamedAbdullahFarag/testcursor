namespace Ikhtibar.Core.DTOs;

/// <summary>
/// Paginated result for user queries
/// Contains users data along with pagination metadata
/// </summary>
public class PagedUserResult
{
    /// <summary>
    /// The users for the current page
    /// </summary>
    public IEnumerable<UserDto> Items { get; set; } = new List<UserDto>();

    /// <summary>
    /// Total number of users matching the query
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// Current page number (1-based)
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; }
}
