using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Document_Manager.Domain.Entities
{
    [Table("Documents")]
    public class Document
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El nombre del archivo es obligatorio")]
        public required string FileName { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        //Relacion con el usuario 
        public Guid UserId { get; set; }
    }
}
