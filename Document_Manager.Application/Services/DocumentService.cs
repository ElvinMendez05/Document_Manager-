
using Document_Manager.Application.DTOs;
using Document_Manager.Application.Interfaces;
using Document_Manager.Domain.Entities;
using Document_Manager.Domain.Interfaces;

namespace Document_Manager.Application.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _repository;
        private readonly IFileStorageService _fileStorageService;

        public DocumentService(IDocumentRepository repository, IFileStorageService fileStorageService)
        {
            _repository = repository;
            _fileStorageService = fileStorageService;
        }

        // Crear documento asociado al usuario
        public async Task<Guid> CreateAsync(CreateDocumentDto dto)
        {
            var document = new Document()
            {
                FileName = dto.FileName,
                FilePath = dto.FilePath,
                UserId = dto.UserId,
            };

            await _repository.AddAsync(document);

            return document.Id;
        }

        // Obtener documentos del usuario
        public async Task<List<DocumentDto>> GetByUserAsync(Guid userId)
        {
            var documents = await _repository.GetByUserAsync(userId);

            return documents.Select(d => new DocumentDto
            {
                Id = d.Id,
                FileName = d.FileName,
                FilePath = d.FilePath,
                CreatedAt = d.CreatedAt
            }).ToList();
        }

        // Obtener documento por ID SOLO si pertenece al usuario
        public async Task<DocumentDto?> GetByIdForUserAsync(Guid id, Guid userId)
        {
            Document? document = await _repository.GetByIdForUserAsync(id, userId);

            if (document is null)
                return null;

            return new DocumentDto
            {
                Id = document.Id,
                FileName = document.FileName,
                FilePath = document.FilePath,
                CreatedAt = document.CreatedAt
            };
        }

        // Eliminar documento SOLO si pertenece al usuario
        public async Task DeleteForUserAsync(Guid id, Guid userId)
        {
            Document document = await _repository.GetByIdForUserAsync(id, userId)
                ?? throw new Exception("Documento no encontrado o no autorizado");

            await _repository.DeleteAsync(document);
        }

        //Ya no es tan util usarlo pero aqui esta 
        public async Task<List<DocumentDto>> GetAllAsync()
        {
            List<Document> documents = await _repository.GetAllAsync();

            return documents.Select(d => new DocumentDto
            {
                Id = d.Id,
                FileName = d.FileName,
                FilePath = d.FilePath,
                CreatedAt = d.CreatedAt
            }).ToList();
        }

        public async Task<(byte[] Content, string FileName)> GetFileForUserAsync(Guid id, Guid userId)
        {
            Document document = await _repository.GetByIdForUserAsync(id, userId)
                ?? throw new Exception("Documento no encontrado");

            byte[] bytes = await _fileStorageService.GetFileAsync(document.FilePath);

            return (bytes, document.FileName);
        }
    }
}




