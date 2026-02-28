using System;
using System.Collections.Generic;

namespace ThesisTestAPI.Entities;

public partial class Material
{
    public Guid MaterialId { get; set; }

    public Guid StepId { get; set; }

    public string Name { get; set; } = null!;

    public int Quantity { get; set; }

    public string UnitOfMeasurement { get; set; } = null!;

    public string Supplier { get; set; } = null!;

    public long Cost { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public virtual Step Step { get; set; } = null!;
}
