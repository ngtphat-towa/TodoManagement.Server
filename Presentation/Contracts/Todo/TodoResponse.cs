namespace Contracts.Todo;

public record TodoResponse
{
    public virtual int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    /// <summary>
    /// Opening = 1, Progressing = 2, Testing = 3, Done = 4, Rejected = 5,
    /// </summary>
    public int Status { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public string LastModifiedBy { get; set; } = string.Empty;
    public DateTime? LastModified { get; set; }
}
