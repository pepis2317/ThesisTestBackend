namespace ThesisTestAPI.Models.User
{
    public class UploadPfpModel
    {
        public required Guid UserId { get; set; }
        public required IFormFile file { get; set; }
    }
}
