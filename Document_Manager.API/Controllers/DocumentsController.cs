using Document_Manager.API.DTOs;
using Document_Manager.Application.DTOs;
using Document_Manager.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Document_Manager.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly IFileStorageService _fileStorage;

        public DocumentsController(IDocumentService documentService, IFileStorageService fileStorage)
        {
            _documentService = documentService;
            _fileStorage = fileStorage;

        }

        // GET: api/documents
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var documents = await _documentService.GetAllAsync();
            return Ok(documents);
        }

        // GET: api/documents/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var document = await _documentService.GetByIdAsync(id);

            if (document == null)
                return NotFound();

            return Ok(document);
        }

        // POST: api/documents
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDocumentDto dto)
        {
            var id = await _documentService.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id }, dto);
        }

        // DELETE: api/documents/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _documentService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] UploadDocumentRequest request)
        {
            // 1️ Validar que venga archivo
            if (request.File == null || request.File.Length == 0)
                return BadRequest("No se ha enviado ningún archivo.");

            // 2️ Lista blanca de extensiones permitidas
            var allowedExtensions = new[] { ".pdf" };
            var fileExtension = Path.GetExtension(request.File.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
                return BadRequest("Solo se permiten archivos PDF.");

            // 3️ Validar Content-Type
            if (request.File.ContentType != "application/pdf")
                return BadRequest("El archivo no es un PDF válido.");

            // 4️ (Opcional) Tamaño máximo (ejemplo: 10MB)
            const long maxFileSize = 10 * 1024 * 1024;
            if (request.File.Length > maxFileSize)
                return BadRequest("El archivo excede el tamaño máximo permitido (10MB).");

            // Guardado del archivo
            var uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                "documents"
            );

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var physicalPath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(physicalPath, FileMode.Create);
            await request.File.CopyToAsync(stream);

            // URL pública
            var publicUrl = $"/uploads/documents/{fileName}";

            // DTO para Application
            var dto = new CreateDocumentDto
            {
                FileName = request.File.FileName,
                FilePath = publicUrl
            };

            var id = await _documentService.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id }, null);
        }
    }
}
