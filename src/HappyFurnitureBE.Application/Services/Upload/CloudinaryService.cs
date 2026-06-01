using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using HappyFurnitureBE.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SkiaSharp;

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

        // Validate file size (max 50MB)
        if (file.Length > 50 * 1024 * 1024)
            throw new ArgumentException("File size cannot exceed 50MB");

        const long cloudinaryLimit = 9 * 1024 * 1024; // 9MB — stays under Cloudinary 10MB plan limit
        Stream uploadStream;

        if (file.Length > cloudinaryLimit)
        {
            // Compress image iteratively until under 9MB
            uploadStream = await CompressImageAsync(file, cloudinaryLimit);
        }
        else
        {
            uploadStream = file.OpenReadStream();
        }

        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(file.FileName, uploadStream),
            Folder = folder,
            Transformation = new Transformation()
                .Quality("auto")
                .FetchFormat("auto"),
            PublicId = $"{folder}_{Guid.NewGuid():N}"
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        await uploadStream.DisposeAsync();

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

    private static Task<Stream> CompressImageAsync(IFormFile file, long targetBytes)
        => Task.FromResult(CompressImageSync(file, targetBytes));

    private static Stream CompressImageSync(IFormFile file, long targetBytes)
    {
        using var sourceStream = file.OpenReadStream();
        var bytes = new byte[sourceStream.Length];
        _ = sourceStream.Read(bytes);

        using var skData = SKData.CreateCopy(bytes);
        using var bitmap = SKBitmap.Decode(skData);

        int quality = 85;
        SKData? encoded;

        do
        {
            encoded = bitmap.Encode(SKEncodedImageFormat.Jpeg, quality);
            if (encoded.Size <= targetBytes || quality <= 20)
                break;

            encoded.Dispose();
            quality -= 10;
        } while (true);

        var output = new MemoryStream(encoded.ToArray());
        encoded.Dispose();
        return output;
    }
}