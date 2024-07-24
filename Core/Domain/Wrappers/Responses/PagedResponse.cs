namespace Shared.Wrappers;

/// <summary>
/// Represents a paged response wrapper for API responses.
/// </summary>
/// <typeparam name="T">Type of the data payload.</typeparam>
public class PagedResponse<T> : Response<T>
{
    /// <summary>
    /// Page number of the current results.
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Number of items per page.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of pages.
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Total number of records across all pages.
    /// </summary>
    public int TotalRecords { get; set; }

    /// <summary>
    /// Constructor for a paged response with data, page number, and page size.
    /// </summary>
    /// <param name="data">The data payload.</param>
    /// <param name="pageNumber">Page number.</param>
    /// <param name="pageSize">Page size.</param>
    public PagedResponse(T data, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        Data = data;
        Message = string.Empty;
        Succeeded = true;
        Errors = default;
    }

    /// <summary>
    /// Constructor for a paged response with data, page number, page size, total pages, and total records.
    /// </summary>
    /// <param name="data">The data payload.</param>
    /// <param name="pageNumber">Page number.</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="totalPages">Total number of pages.</param>
    /// <param name="totalRecords">Total number of records across all pages.</param>
    public PagedResponse(T data, int pageNumber, int pageSize, int totalPages, int totalRecords)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        Data = data;
        TotalPages = totalPages;
        TotalRecords = totalRecords;
        Message = string.Empty;
        Succeeded = true;
        Errors = default;
    }
}