using Document_Manager.API.DTOs;
using Document_Manager.Application.DTOs;
using Document_Manager.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Document_Manager.API.Controllers
{
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
            var uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                "documents"
            );

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.File.FileName)}";
            var physicalPath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(physicalPath, FileMode.Create);
            await request.File.CopyToAsync(stream);

            var publicUrl = $"/uploads/documents/{fileName}";

            var dto = new CreateDocumentDto
            {
                FileName = request.File.FileName,
                FilePath = publicUrl
            };

            var id = await _documentService.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id }, null);
        }


        //// POST: api/documents/upload
        //[HttpPost("upload")]
        //public async Task<IActionResult> Upload([FromForm] UploadDocumentRequest request)
        //{
        //    var uploadsFolder = Path.Combine("Uploads", "Documents");

        //    if (!Directory.Exists(uploadsFolder))
        //        Directory.CreateDirectory(uploadsFolder);

        //    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.File.FileName)}";
        //    var filePath = Path.Combine(uploadsFolder, fileName);

        //    using var stream = new FileStream(filePath, FileMode.Create);
        //    await request.File.CopyToAsync(stream);

        //    var dto = new CreateDocumentDto
        //    {
        //        FileName = request.File.FileName,
        //        FilePath = filePath
        //    };

        //    var id = await _documentService.CreateAsync(dto);

        //    return CreatedAtAction(nameof(GetById), new { id }, null);
        //}
    }
}
