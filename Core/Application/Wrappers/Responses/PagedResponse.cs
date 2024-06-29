namespace Shared.Wrappers;

public class PagedResponse<T>
{
    public List<T> Data { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }

    public PagedResponse(IQueryable<T> query, PaginationFilter pagination)
    {
        PageNumber = pagination.PageNumber;
        PageSize = pagination.PageSize;

        TotalRecords = query.Count();

        // Calculate total pages
        TotalPages = PageSize > 0 ? (int)System.Math.Ceiling(TotalRecords / (double)PageSize) : 0;

        // Apply pagination to query
        if (pagination.Skip.HasValue)
        {
            query = query.Skip(pagination.Skip.Value);
        }

        if (pagination.PageSize > 0)
        {
            Data = query.Skip((pagination.PageNumber - 1) * pagination.PageSize)
                        .Take(pagination.PageSize)
                        .ToList();
        }
        else
        {
            Data = query.ToList();
        }
    }
}

