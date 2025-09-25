using System;
using System.Collections.Generic;

namespace ThesisTestAPI.Entities;

public partial class SellerReview
{
    public Guid SellerReviewId { get; set; }

    public Guid SellerId { get; set; }

    public string Review { get; set; } = null!;

    public virtual Seller Seller { get; set; } = null!;

    public virtual Content SellerReviewNavigation { get; set; } = null!;
}
