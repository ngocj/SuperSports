using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SP.Application.Service.Interface;

namespace SP.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageFileController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageFileController(IImageService imageService)
        {
            _imageService = imageService;
        }
        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] int productVariantId)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            await _imageService.UploadFileAsync(file, productVariantId);
            return Ok(new { message = "File uploaded successfully." });
        }
        [HttpGet("Download")]
        public async Task<IActionResult> Download(int id)
        {
            var fileDto = await _imageService.DownloadFile(id);
            if (fileDto == null)
            {
                return NotFound();
            }
            return File(fileDto.FileData, fileDto.ContentType, fileDto.FileName);
        }

        [HttpGet]
        public async Task<IActionResult> GetListFile()
        {
            var files = await _imageService.GetAllFileAsync();
            return Ok(files);
        }   
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFile(int id)
        {
            var fileDto = await _imageService.DownloadFile(id);
            if (fileDto == null)
            {
                return NotFound();
            }
            return File(fileDto.FileData, fileDto.ContentType, fileDto.FileName);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var fileDto = await _imageService.DownloadFile(id);
            if (fileDto == null)
            {
                return NotFound();
            }
            await _imageService.DeleteFileAsync(id);
            return Ok(new { message = "File deleted successfully." });
        }

    }
}
