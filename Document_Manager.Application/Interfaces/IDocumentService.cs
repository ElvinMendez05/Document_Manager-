using Document_Manager.Application.DTOs;

namespace Document_Manager.Application.Interfaces
{
    public interface IDocumentService
    {
        Task<int> CreateAsync(CreateDocumentDto dto);
        Task<List<DocumentDto>> GetAllAsync();
        Task<DocumentDto?> GetByIdAsync(int id);
        Task DeleteAsync(int id);
    }
}
