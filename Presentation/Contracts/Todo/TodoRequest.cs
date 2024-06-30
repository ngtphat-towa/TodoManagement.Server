namespace Contracts.Todo;

public record CreateTodoRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    /// <summary>
    /// Opening = 1, Progressing = 2, Testing = 3, Done = 4, Rejected = 5,
    /// </summary>
    public int Status { get; set; }
}
public record UpdateTodoRequest : CreateTodoRequest
{
    public int Id { get;set; }
}
public record DeleteTodoRequest
{
    public int Id { get; set; }
}

public record GetAllTodosQuery
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}