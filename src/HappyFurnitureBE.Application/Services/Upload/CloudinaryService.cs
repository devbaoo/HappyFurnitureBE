using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using HappyFurnitureBE.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace HappyFurnitureBE.Application.Services.Upload;

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IConfiguration configuration)
    {
        var cloudinarySettings = configuration.GetSection("Cloudinary");
        var account = new Account(
            cloudinarySettings["CloudName"],
            cloudinarySettings["ApiKey"],
            cloudinarySettings["ApiSecret"]
        );
        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> UploadImageAsync(IFormFile file, string folder = "products")
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty or null");

        // Validate file type
        var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
        if (!allowedTypes.Contains(file.ContentType.ToLower()))
            throw new ArgumentException("Invalid file type. Only JPEG, PNG, GIF, and WebP are allowed.");

        // Validate file size (max 10MB)
        if (file.Length > 10 * 1024 * 1024)
            throw new ArgumentException("File size cannot exceed 10MB");

        using var stream = file.OpenReadStream();
        
        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(file.FileName, stream),
            Folder = folder,
            Transformation = new Transformation()
                .Quality("auto")
                .FetchFormat("auto"),
            PublicId = $"{folder}_{Guid.NewGuid():N}"
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null)
            throw new Exception($"Cloudinary upload error: {uploadResult.Error.Message}");

        return uploadResult.SecureUrl.ToString();
    }

    public async Task<bool> DeleteImageAsync(string publicId)
    {
        if (string.IsNullOrEmpty(publicId))
            return false;

        var deleteParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deleteParams);
        
        return result.Result == "ok";
    }

    public async Task<List<string>> UploadMultipleImagesAsync(List<IFormFile> files, string folder = "products")
    {
        if (files == null || !files.Any())
            throw new ArgumentException("No files provided");

        var uploadTasks = files.Select(file => UploadImageAsync(file, folder));
        var results = await Task.WhenAll(uploadTasks);
        
        return results.ToList();
    }
}