using Document_Manager.Domain.Entities;

namespace Document_Manager.Domain.Interfaces
{
    public interface IDocumentRepository
    {
        Task<Document?> GetByIdAsync(int id);
        Task<List<Document>> GetAllAsync();
        Task AddAsync(Document document);
        Task DeleteAsync(Document document);

    }
}
