namespace Document_Manager.API.DTOs
{
    public class UploadDocumentRequest
    {
        public IFormFile File { get; set; } = null!;
    }
}
