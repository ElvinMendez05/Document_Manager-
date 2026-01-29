
namespace Document_Manager.Application.DTOs
{
    public class CreateDocumentDto
    {
        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public Guid UserId { get; set; }
    }
}
