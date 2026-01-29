using Document_Manager.Domain.Entities;

namespace Document_Manager.Domain.Interfaces
{
    public interface IDocumentRepository
    {
        
        //Interface para relacionar el usuario  
        Task<List<Document>> GetByUserIdAsync(int id, Guid userId);

        Task AddAsync(Document document);
        Task DeleteAsync(Document document);

        Task<List<Document>> GetAllAsync(); // opcional
        Task<List<Document>> GetByUserAsync(Guid userId);
        Task<Document?> GetByIdForUserAsync(int id, Guid userId);
    }
}
