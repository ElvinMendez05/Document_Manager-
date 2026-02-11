using Document_Manager.API.DTOs;
using Document_Manager.Application.DTOs;
using Document_Manager.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        // GET: api/documents/my-documents
        // Devuelve SOLO los documentos del usuario autenticado
        [HttpGet("my-documents")]
        public async Task<IActionResult> GetMyDocuments()
        {
            var userId = Guid.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var documents = await _documentService.GetByUserAsync(userId);
            return Ok(documents);
        }

        // GET: api/documents/{id}
        // Obtiene un documento SOLO si pertenece al usuario
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userId = Guid.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var document = await _documentService.GetByIdForUserAsync(id, userId);

            if (document == null)
                return NotFound();

            return Ok(document);
        }


        // POST: api/documents/upload
        [Authorize]
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] UploadDocumentRequest request)
        {
            // Validar archivo
            if (request.File == null || request.File.Length == 0)
                return BadRequest("No se ha enviado ningún archivo.");

            // Validar extensión
            var allowedExtensions = new[] { ".pdf" };
            var fileExtension = Path.GetExtension(request.File.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
                return BadRequest("Solo se permiten archivos PDF.");

            // Validar Content-Type
            if (request.File.ContentType != "application/pdf")
                return BadRequest("El archivo no es un PDF válido.");

            // Tamaño máximo (10MB)
            const long maxFileSize = 10 * 1024 * 1024;
            if (request.File.Length > maxFileSize)
                return BadRequest("El archivo excede el tamaño máximo permitido (10MB).");

            // Obtener UserId desde el JWT
            var userId = Guid.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            // Guardar archivo físico
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

            // Crear DTO para Application
            var dto = new CreateDocumentDto
            {
                FileName = request.File.FileName,
                FilePath = publicUrl,
                UserId = userId
            };

            var id = await _documentService.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id }, null);
        }


        // DELETE: api/documents/{id} 
        [AllowAnonymous]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = Guid.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            await _documentService.DeleteForUserAsync(id, userId);
            return NoContent();
        }

    }
}
