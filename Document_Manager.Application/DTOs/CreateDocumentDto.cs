
using System.ComponentModel.DataAnnotations;

namespace Document_Manager.Application.DTOs
{
    public class CreateDocumentDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public required string FileName { get; set; } 
        public required string FilePath { get; set; } 
        public Guid UserId { get; set; }
    }
}
