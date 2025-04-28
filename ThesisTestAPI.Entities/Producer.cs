using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace ThesisTestAPI.Entities;

public partial class Producer
{
    public Guid ProducerId { get; set; }

    public string ProducerName { get; set; } = null!;

    public Guid OwnerId { get; set; }

    public int? Rating { get; set; }

    public int? Clients { get; set; }

    public Geometry Location { get; set; } = null!;

    public string? Banner { get; set; }

    public virtual User Owner { get; set; } = null!;
}
