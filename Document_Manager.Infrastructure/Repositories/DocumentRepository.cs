using Document_Manager.Domain.Entities;
using Document_Manager.Domain.Interfaces;
using Document_Manager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Document_Manager.Infrastructure.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly AppDbContext _context;

        public DocumentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Document document)
        {
            _context.Documents.Add(document);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Document>> GetAllAsync()
        {
            return await _context.Documents.ToListAsync();
        }

        public async Task<Document?> GetByIdAsync(int id)
        {
            return await _context.Documents.FindAsync(id);
        }

        public async Task DeleteAsync(Document document)
        {
            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
        }
    }
}
