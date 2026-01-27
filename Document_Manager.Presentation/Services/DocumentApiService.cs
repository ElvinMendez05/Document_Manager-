using System.Linq.Expressions;
using System.Net.Http.Headers;
using Document_Manager.Presentation.Model;
using Microsoft.AspNetCore.Components.Forms;

namespace Document_Manager.Presentation.Services
    {
        public class DocumentApiService
        {
            private readonly HttpClient _http;

            public DocumentApiService(HttpClient http)
            {
                _http = http;
            }

            public async Task<List<DocumentViewModel>> GetAllAsync()
            {
                try
                {
                    return await _http.GetFromJsonAsync<List<DocumentViewModel>>("api/documents")
                           ?? new List<DocumentViewModel>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener documentos: {ex.Message}");
                    return new List<DocumentViewModel>();
                }
            }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var response = await _http.DeleteAsync($"api/documents/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar documento: {ex.Message}");
                return false;
            }
        }

        public async Task UploadAsync(IBrowserFile file)
                {
                    using var content = new MultipartFormDataContent();

                    var fileContent = new StreamContent(file.OpenReadStream(10 * 1024 * 1024));
                    fileContent.Headers.ContentType =
                        new MediaTypeHeaderValue(file.ContentType);

                    content.Add(fileContent, "File", file.Name);

                    var response = await _http.PostAsync("api/documents/upload", content);

                    response.EnsureSuccessStatusCode();
                }
        }
    }
