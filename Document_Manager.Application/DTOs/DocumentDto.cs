namespace Document_Manager.Application.DTOs
{
    public class DocumentDto
    {
        public Guid Id { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; } 
        public DateTime CreatedAt { get; set; }
    }
}
