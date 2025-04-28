using System;
using System.Collections.Generic;

namespace ThesisTestAPI.Entities;

public partial class DataProtectionKey
{
    public int Id { get; set; }

    public string XmlData { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }
}
