namespace ThesisTestAPI.Models.User
{
    public class PaginatedUserResponse
    {
        public int? Total { get; set; }
        public List<UserResponse>? Users { get; set; }
    }
}
