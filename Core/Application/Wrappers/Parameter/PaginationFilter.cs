namespace Shared.Wrappers;

public class PaginationFilter
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int? Skip { get; set; }

    public PaginationFilter()
    {
        PageNumber = 1;
        PageSize = 10; 
    }

    public PaginationFilter(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber < 1 ? 1 : pageNumber;
        PageSize = pageSize;
    }
}


public class DataFilter
{
    public string? FileName { get; set; }
    public string? Type { get; set; }
    public string? Value { get; set; }
}


public class DataSort
{
    public string? Selector { get; set; }
    public bool? Descending { get; set; }
}
