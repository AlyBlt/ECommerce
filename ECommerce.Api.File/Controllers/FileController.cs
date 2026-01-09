using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace ECommerce.Api.File.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly string _uploadPath;

        public FileController()
        {
            // Klasör yolunu belirle
            _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            // Eğer ana klasör yoksa oluştur
            if (!Directory.Exists(_uploadPath))
                Directory.CreateDirectory(_uploadPath);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            // 1. Dosya kontrolü
            if (file == null || file.Length == 0)
                return BadRequest("The file was not selected.");

            // 2. Güvenli ve benzersiz dosya adı oluşturma
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(_uploadPath, fileName);

            // 3. Dosyayı fiziksel olarak kaydetme
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Ödevin istediği: 201 Created ve Response Body'de dosya adı
            // URL kısmını boş bırakabilir veya indirme linkini verebilirsin
            return Created("", new { fileName = fileName });
        }

        [HttpGet("download")]
        public IActionResult Download([FromQuery] string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return BadRequest("The file name is not specified.");

            var filePath = Path.Combine(_uploadPath, fileName);

            // Dosya var mı kontrolü
            if (!System.IO.File.Exists(filePath))
                return NotFound("The file could not be found in the system.");

            // MIME Type (İçerik türü) belirleme (Ör: image/png)
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var contentType))
            {
                contentType = "application/octet-stream"; // Tür bulunamazsa genel indirilebilir dosya türü
            }

            var bytes = System.IO.File.ReadAllBytes(filePath);

            // Dosyayı tarayıcıya fırlatıyoruz
            return File(bytes, contentType, fileName);
        }
    }
}