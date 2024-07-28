namespace Shared.Wrappers;

public class PaginationFilter
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public PaginationFilter()
    {
        PageNumber = 1;
        PageSize = 10;
    }

    public PaginationFilter(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber < 1 ? 1 : pageNumber;
        PageSize = pageSize < 1 ? 10 : pageSize;
    }
}
public record RecordPagination
{
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }

}


public class DataFilter
{
    public string? PropertyName { get; set; }
    public string? PropertyValue { get; set; }
}


public class DataSort
{
    public string? SortBy { get; set; }
    public bool IsDescending { get; set; }
}
