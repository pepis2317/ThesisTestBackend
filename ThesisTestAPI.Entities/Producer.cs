using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace ThesisTestAPI.Entities;

public partial class Producer
{
    public Guid ProducerId { get; set; }

    public string ProducerName { get; set; } = null!;

    public Guid OwnerId { get; set; }

    public Geometry? Location { get; set; }

    public string? Banner { get; set; }

    public string? ProducerPicture { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User Owner { get; set; } = null!;

    public virtual ICollection<ProducerReview> ProducerReviews { get; set; } = new List<ProducerReview>();

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
