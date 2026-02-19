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
        public DateTime CreatedAt { get; set; }

        //Relacion con el usuario 
        public Guid UserId { get; set; }
        //protected Document() { } 

        //public Document(string fileName, string filePath, Guid userId)
        //{

        //    if (string.IsNullOrWhiteSpace(fileName))
        //        throw new ArgumentException("El nombre del archivo es obligatorio");

        //    if (string.IsNullOrWhiteSpace(filePath))
        //        throw new ArgumentException("La ruta del archivo es obligatoria");

        //    FileName = fileName;
        //    FilePath = filePath;
        //    CreatedAt = DateTime.UtcNow;
        //    UserId = userId;
           
        //}
    }
}
