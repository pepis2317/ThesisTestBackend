namespace ThesisTestAPI.Models.User
{
    public class UserRegisterModel
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }

    }
}
