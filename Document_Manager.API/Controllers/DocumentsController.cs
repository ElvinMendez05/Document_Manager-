using Document_Manager.API.DTOs;
using Document_Manager.Application.DTOs;
using Document_Manager.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Document_Manager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly IFileStorageService _fileStorage;
        
        public DocumentsController(IDocumentService documentService, 
                                   IFileStorageService fileStorage)
        {
            _documentService = documentService;
            _fileStorage = fileStorage;
        }

        // GET: api/documents/my-documents
        // Devuelve SOLO los documentos del usuario autenticado
        [Authorize(Roles = "User")]
        [HttpGet("my-documents")]
        public async Task<IActionResult> GetMyDocuments()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized("No se pudo obtener el usuario del token.");

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("Identificador de usuario inválido.");

            var documents = await _documentService.GetByUserAsync(userId);

            return Ok(documents);
        }

        // GET: api/documents/{id}
        // Obtiene un documento SOLO si pertenece al usuario
        [Authorize(Roles = "User")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized("No se pudo obtener el usuario del token.");

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("Identificador de usuario inválido.");

            var document = await _documentService.GetByIdForUserAsync(id, userId);

            if (document == null)
                return NotFound();

            return Ok(document);
        }

        //Preview para el modal 
        [Authorize(Roles = "User")]
        [HttpGet("{id}/preview")]
        public async Task<IActionResult> Preview(Guid id)
        {
            var userId = Guid.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var document = await _documentService.GetByIdForUserAsync(id, userId);

            if (document == null)
                return NotFound();

            var physicalPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                document.FilePath!.TrimStart('/')
            );

            if (!System.IO.File.Exists(physicalPath))
                return NotFound();

            var bytes = await System.IO.File.ReadAllBytesAsync(physicalPath);

            // 🔥 Detectar Content-Type real
            var extension = Path.GetExtension(physicalPath).ToLowerInvariant();

            var contentType = extension switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".ppt" => "application/vnd.ms-powerpoint",
                ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".zip" => "application/zip",
                ".rar" => "application/x-rar-compressed",
                _ => "application/octet-stream"
            };

            return File(bytes, contentType);
        }

        // POST: api/documents/upload
        [Authorize(Roles = "User")]
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] UploadDocumentRequest request)
        {
            // Validar archivo
            if (request.File == null || request.File.Length == 0)
                return BadRequest("No se ha enviado ningún archivo.");

            // Validar extensión
            var allowedExtensions = new[] { 
                // Documentos
                   ".pdf", ".doc", ".docx", ".txt", ".rtf", ".odt",

                // Hojas de cálculo
                   ".xls", ".xlsx", ".csv", ".ods",

                // Presentaciones
                   ".ppt", ".pptx", ".odp",

                // Imágenes
                   ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg",

                // Comprimidos
               ".zip", ".rar", ".7z" };

            var fileExtension = Path.GetExtension(request.File.FileName)
                                    .ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
                return BadRequest("Tipo de archivo no permitido.");

            var allowedContentTypes = new[]
               {
                 // PDF
                  "application/pdf",

                 // Word
                  "application/msword",
                  "application/vnd.openxmlformats-officedocument.wordprocessingml.document",

                 // Excel
                  "application/vnd.ms-excel",
                  "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",

                 // PowerPoint
                  "application/vnd.ms-powerpoint",
                  "application/vnd.openxmlformats-officedocument.presentationml.presentation",

                 // Texto
                  "text/plain",
                  "text/csv",

                 // Imágenes
                  "image/jpeg",
                  "image/png",
                  "image/gif",
                  "image/webp",
                  "image/svg+xml",

                 // Comprimidos
                  "application/zip",
                  "application/x-rar-compressed",
                  "application/x-7z-compressed"
            };

            // Validar Content-Type
            if (!allowedContentTypes.Contains(request.File.ContentType))
                return BadRequest("Content-Type no permitido.");

            // Tamaño máximo (10MB)
            const long maxFileSize = 10 * 1024 * 1024;
            if (request.File.Length > maxFileSize)
                return BadRequest("El archivo excede el tamaño máximo permitido (10MB).");

            // Obtener UserId desde el JWT de forma segura
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized("No se pudo obtener el usuario desde el token.");

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("El identificador del usuario no es válido.");

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

            using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                await request.File.CopyToAsync(stream);
            }

            // URL pública
            var publicUrl = $"/uploads/documents/{fileName}";

            // Crear DTO
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
        [Authorize(Roles = "User")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized("No se pudo obtener el usuario del token.");

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized("Identificador de usuario inválido.");

            await _documentService.DeleteForUserAsync(id, userId);

            return NoContent();
        }
    }
}
