using Document_Manager.Application.DTOs;

namespace Document_Manager.Application.Interfaces
{
    public interface IDocumentService
    {
        Task<Guid> CreateAsync(CreateDocumentDto dto);
        Task<List<DocumentDto>> GetAllAsync();
        Task<List<DocumentDto>> GetByUserAsync(Guid userId);
        Task DeleteForUserAsync(Guid id, Guid userId);
        Task<DocumentDto?> GetByIdForUserAsync(Guid id, Guid userId);
    }
}
