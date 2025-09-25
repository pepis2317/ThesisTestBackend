using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace ThesisTestAPI.Entities;

public partial class Seller
{
    public Guid SellerId { get; set; }

    public string SellerName { get; set; } = null!;

    public Guid OwnerId { get; set; }

    public Geometry? Location { get; set; }

    public string? Banner { get; set; }

    public string? SellerPicture { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User Owner { get; set; } = null!;

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    public virtual ICollection<SellerReview> SellerReviews { get; set; } = new List<SellerReview>();
}
