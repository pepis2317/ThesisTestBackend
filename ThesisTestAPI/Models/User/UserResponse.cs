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
    }
}
