namespace ThesisTestAPI.Models.MessageAttachments
{
    public class MessageAttachmentResponse
    {
        public Guid AttachmentId {  get; set; }
        public string BlobFileName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string ContentType {  get; set; } = string.Empty;
        public long Size { get; set; }
    }
}
