using Document_Manager.Domain.Entities;

namespace Document_Manager.Domain.Interfaces
{
    public interface IDocumentRepository
    {
        
        //Interface para relacionar el usuario  
        Task<List<Document>> GetByUserIdAsync(Guid id, Guid userId);

        Task AddAsync(Document document);
        Task DeleteAsync(Document document);

        Task<List<Document>> GetAllAsync(); // opcional
        Task<List<Document>> GetByUserAsync(Guid userId);
        Task<Document?> GetByIdForUserAsync(Guid id, Guid userId);
    }
}
