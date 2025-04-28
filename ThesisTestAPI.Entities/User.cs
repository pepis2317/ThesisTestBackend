using System;
using System.Collections.Generic;

namespace ThesisTestAPI.Entities;

public partial class User
{
    public Guid UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int? Rating { get; set; }

    public string? Pfp { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }

    public string? Role { get; set; }

    public virtual ICollection<Producer> Producers { get; set; } = new List<Producer>();
}
