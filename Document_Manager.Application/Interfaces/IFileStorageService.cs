namespace Document_Manager.Application.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveAsync(Stream fileStream, string fileName);
        Task<byte[]> GetFileAsync(string path);
    }
}
