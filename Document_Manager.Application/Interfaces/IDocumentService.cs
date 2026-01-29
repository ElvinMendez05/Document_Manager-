using Document_Manager.Application.DTOs;

namespace Document_Manager.Application.Interfaces
{
    public interface IDocumentService
    {
        Task<int> CreateAsync(CreateDocumentDto dto);
        Task<List<DocumentDto>> GetAllAsync();

        //Interface de relacion con el usuario 
        Task<List<DocumentDto>> GetByUserAsync(Guid userId);
        Task DeleteForUserAsync(int id, Guid userId);
        Task<DocumentDto?> GetByIdForUserAsync(int id, Guid userId);
    }
}
