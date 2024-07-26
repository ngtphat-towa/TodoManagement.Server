namespace Domain.Common;

public abstract class AuditableBaseEntity : BaseEntity
{
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public string LastModifiedBy { get; set; } = string.Empty;
    public DateTime? LastModified { get; set; }
}