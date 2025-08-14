namespace Ikhtibar.Shared.Models;

/// <summary>
/// Generic paginated result container for API responses
/// </summary>
/// <typeparam name="T">Type of items in the result</typeparam>
public class PaginatedResult<T>
{
    /// <summary>
    /// List of items for the current page
    /// </summary>
    public IEnumerable<T> Items { get; set; } = new List<T>();

    /// <summary>
    /// Current page number (1-based)
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of items across all pages
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>
    /// Whether there is a next page
    /// </summary>
    public bool HasNextPage => Page < TotalPages;

    /// <summary>
    /// Creates a new paginated result
    /// </summary>
    /// <param name="items">Items for the current page</param>
    /// <param name="totalCount">Total number of items</param>
    /// <param name="page">Current page number</param>
    /// <param name="pageSize">Items per page</param>
    public PaginatedResult(IEnumerable<T> items, int totalCount, int page, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
    }

    /// <summary>
    /// Creates an empty paginated result
    /// </summary>
    public PaginatedResult()
    {
    }
}
