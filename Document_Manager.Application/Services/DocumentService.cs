
using Document_Manager.Application.DTOs;
using Document_Manager.Application.Interfaces;
using Document_Manager.Domain.Entities;
using Document_Manager.Domain.Interfaces;

namespace Document_Manager.Application.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _repository;

        public DocumentService(IDocumentRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> CreateAsync(CreateDocumentDto dto)
        {
            var document = new Document(dto.FileName, dto.FilePath);

            await _repository.AddAsync(document);

            return document.Id;
        }

        public async Task<List<DocumentDto>> GetAllAsync()
        {
            var documents = await _repository.GetAllAsync();

            return documents.Select(d => new DocumentDto
            {
                Id = d.Id,
                FileName = d.FileName,
                FilePath = d.FilePath,
                CreatedAt = d.CreatedAt
            }).ToList();
        }

        public async Task<DocumentDto?> GetByIdAsync(int id)
        {
            var document = await _repository.GetByIdAsync(id);

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

        public async Task DeleteAsync(int id)
        {
            var document = await _repository.GetByIdAsync(id);

            if (document is null)
                throw new Exception("Documento no encontrado");

            await _repository.DeleteAsync(document);
        }
    }
}
