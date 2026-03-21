using HappyFurnitureBE.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HappyFurnitureBE.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin")]
public class UploadController : ControllerBase
{
    private readonly ICloudinaryService _cloudinaryService;
    private readonly ILogger<UploadController> _logger;

    public UploadController(ICloudinaryService cloudinaryService, ILogger<UploadController> logger)
    {
        _cloudinaryService = cloudinaryService;
        _logger = logger;
    }

    /// <summary>
    /// Upload single image to Cloudinary
    /// Supported folders: products, categories, product-variants, product-images
    /// </summary>
    [HttpPost("image")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<UploadImageResponse>> UploadImage(IFormFile file, [FromQuery] string folder = "products")
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "No file provided" });
            }

            // Validate folder
            var allowedFolders = new[] { "products", "categories", "product-variants", "product-images" };
            if (!allowedFolders.Contains(folder.ToLower()))
            {
                return BadRequest(new { message = $"Invalid folder. Allowed folders: {string.Join(", ", allowedFolders)}" });
            }

            var imageUrl = await _cloudinaryService.UploadImageAsync(file, folder);
            
            return Ok(new UploadImageResponse
            {
                ImageUrl = imageUrl,
                FileName = file.FileName,
                FileSize = file.Length,
                ContentType = file.ContentType
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid file upload attempt");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while uploading image");
            return StatusCode(500, new { message = "Internal server error during upload" });
        }
    }

    /// <summary>
    /// Upload multiple images to Cloudinary
    /// Supported folders: products, categories, product-variants, product-images
    /// </summary>
    [HttpPost("images")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<UploadMultipleImagesResponse>> UploadMultipleImages(List<IFormFile> files, [FromQuery] string folder = "products")
    {
        try
        {
            if (files == null || !files.Any())
            {
                return BadRequest(new { message = "No files provided" });
            }

            if (files.Count > 10)
            {
                return BadRequest(new { message = "Maximum 10 files allowed per upload" });
            }

            // Validate folder
            var allowedFolders = new[] { "products", "categories", "product-variants", "product-images" };
            if (!allowedFolders.Contains(folder.ToLower()))
            {
                return BadRequest(new { message = $"Invalid folder. Allowed folders: {string.Join(", ", allowedFolders)}" });
            }

            var imageUrls = await _cloudinaryService.UploadMultipleImagesAsync(files, folder);
            
            return Ok(new UploadMultipleImagesResponse
            {
                ImageUrls = imageUrls,
                TotalFiles = files.Count,
                TotalSize = files.Sum(f => f.Length)
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid files upload attempt");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while uploading multiple images");
            return StatusCode(500, new { message = "Internal server error during upload" });
        }
    }

    /// <summary>
    /// Delete image from Cloudinary
    /// </summary>
    [HttpDelete("image")]
    public async Task<ActionResult> DeleteImage([FromQuery] string publicId)
    {
        try
        {
            if (string.IsNullOrEmpty(publicId))
            {
                return BadRequest(new { message = "Public ID is required" });
            }

            var result = await _cloudinaryService.DeleteImageAsync(publicId);
            
            if (result)
            {
                return Ok(new { message = "Image deleted successfully" });
            }
            else
            {
                return NotFound(new { message = "Image not found or already deleted" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting image with publicId: {PublicId}", publicId);
            return StatusCode(500, new { message = "Internal server error during deletion" });
        }
    }
}

public class UploadImageResponse
{
    public string ImageUrl { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
}

public class UploadMultipleImagesResponse
{
    public List<string> ImageUrls { get; set; } = new();
    public int TotalFiles { get; set; }
    public long TotalSize { get; set; }
}