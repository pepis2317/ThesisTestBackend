using System;
using System.Collections.Generic;

namespace ThesisTestAPI.Entities;

public partial class Request
{
    public Guid RequestId { get; set; }

    public Guid SellerId { get; set; }

    public string RequestMessage { get; set; } = null!;

    public string RequestStatus { get; set; } = null!;

    public string RequestTitle { get; set; } = null!;

    public virtual ICollection<Process> Processes { get; set; } = new List<Process>();

    public virtual Content RequestNavigation { get; set; } = null!;

    public virtual Seller Seller { get; set; } = null!;
}
