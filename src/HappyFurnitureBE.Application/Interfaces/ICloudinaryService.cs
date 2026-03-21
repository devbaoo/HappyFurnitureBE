using Microsoft.AspNetCore.Http;

namespace HappyFurnitureBE.Application.Interfaces;

public interface ICloudinaryService
{
    Task<string> UploadImageAsync(IFormFile file, string folder = "products");
    Task<bool> DeleteImageAsync(string publicId);
    Task<List<string>> UploadMultipleImagesAsync(List<IFormFile> files, string folder = "products");
}