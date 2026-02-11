namespace Document_Manager.Domain.Entities
{
    public class Document
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        //Relacion con el usuario 
        public Guid UserId { get; set; }

        protected Document() { } 

        public Document(string fileName, string filePath, Guid userId)
        {

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("El nombre del archivo es obligatorio");

            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("La ruta del archivo es obligatoria");

            FileName = fileName;
            FilePath = filePath;
            CreatedAt = DateTime.UtcNow;
            UserId = userId;
        }
    }
}
