namespace ThesisTestAPI.Models.User
{
    public class UserResponse
    {
        public Guid? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public int? Rating { get; set; }
        public string? Pfp { get; set; }
        public string? Role { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Address { get; set; }
        public int? PostalCode { get; set; }
    }
}
