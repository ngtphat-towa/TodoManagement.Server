namespace Shared.Wrappers;

public class PagedResponse<T> : Response<T>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }

    public PagedResponse(T data, int pageNumber, int pageSize)
    {
        this.PageNumber = pageNumber;
        this.PageSize = pageSize;
        this.Data = data;
        this.Message = string.Empty;
        this.Succeeded = true;
        this.Errors = default;
    }
    public PagedResponse(T data, int pageNumber, int pageSize, int totalPages, int totalRecords)
    {
        this.PageNumber = pageNumber;
        this.PageSize = pageSize;
        this.Data = data;
        this.TotalPages = totalPages;
        this.TotalRecords = totalRecords;
        this.Message = string.Empty;
        this.Succeeded = true;
        this.Errors = default;
    }
}

