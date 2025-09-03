using Microsoft.AspNetCore.Http;
using SP.Application.Dto.ImageDto;
using SP.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Application.Service.Interface
{
    public interface IImageService
    {
        Task UploadFileAsync(IFormFile formFile, int productVariantId);
        Task<ImageFileDto> DownloadFile(int id);
        Task<List<ImageFileDto>> GetAllFileAsync();
        Task DeleteFileAsync(int id);
        Task EditFileAsync(int id, IFormFile formFile);

    }
}
