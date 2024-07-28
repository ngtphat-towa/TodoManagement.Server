﻿using Domain.Common;

namespace Domain.Entities;

public class Todo : AuditableBaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    /// <summary>
    /// Opening = 1, Progressing = 2, Testing = 3, Done = 4, Rejected = 5,
    /// </summary>
    public short Status { get; set; } = (short)TodoStatusEnum.Opening;
}