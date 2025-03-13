namespace ThesisTestAPI.Models.User
{
    public class UserEditModel
    {
        public required Guid UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email {  get; set; }
        public string? Password { get; set; }
        public string? Phone {  get; set; }
    }
}
