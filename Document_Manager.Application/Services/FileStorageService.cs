

using Document_Manager.Application.Interfaces;

namespace Document_Manager.Application.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _basePath = Path.Combine("Uploads", "Documents");

        public async Task<string> SaveAsync(Stream fileStream, string fileName)
        {
            if (!Directory.Exists(_basePath))
                Directory.CreateDirectory(_basePath);

            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
            var fullPath = Path.Combine(_basePath, uniqueFileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await fileStream.CopyToAsync(stream);

            return fullPath;
        }

        public async Task<byte[]> GetFileAsync(string path)
        {
            return await File.ReadAllBytesAsync(path);
        }
    }
}
