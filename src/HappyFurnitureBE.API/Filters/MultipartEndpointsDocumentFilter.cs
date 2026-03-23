using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HappyFurnitureBE.API.Filters;

/// <summary>
/// Thêm thủ công các endpoint multipart vào Swagger (vì Swagger không tự generate được IFormFile).
/// </summary>
public class MultipartEndpointsDocumentFilter : IDocumentFilter
{
    private static OpenApiSecurityRequirement BearerAuth => new()
    {
        { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, Array.Empty<string>() }
    };

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        // 1. POST /api/Categories/with-image
        swaggerDoc.Paths["/api/Categories/with-image"] = new OpenApiPathItem
        {
            Operations = new Dictionary<OperationType, OpenApiOperation>
            {
                [OperationType.Post] = CreateMultipartOp(
                    "Tạo category với ảnh (multipart)",
                    "Fields: name (required), parentId (optional), isActive (optional), image (optional, file)",
                    "Categories",
                    new Dictionary<string, OpenApiSchema>
                    {
                        ["name"] = new() { Type = "string", Description = "Tên category" },
                        ["parentId"] = new() { Type = "integer", Nullable = true, Description = "ID category cha (để trống nếu là category gốc)" },
                        ["isActive"] = new() { Type = "boolean", Description = "Trạng thái active" },
                        ["image"] = new() { Type = "string", Format = "binary", Description = "File ảnh" }
                    },
                    required: new[] { "name" })
            }
        };

        // 2. POST /api/Products/with-images
        swaggerDoc.Paths["/api/Products/with-images"] = new OpenApiPathItem
        {
            Operations = new Dictionary<OperationType, OpenApiOperation>
            {
                [OperationType.Post] = CreateMultipartOp(
                    "Tạo product với nhiều ảnh (multipart)",
                    "Fields: name, slug, price (bắt buộc). Các field tuỳ chọn: description, oldPrice, dimensionsHeight, dimensionsWidth, dimensionsDepth, dimensionUnit, detail, deliveryInfo, weight, isFeatured, isActive, categoryIds (phân cách bằng dấu phẩy), images (files)",
                    "Products",
                    new Dictionary<string, OpenApiSchema>
                    {
                        ["name"] = new() { Type = "string", Description = "Tên product" },
                        ["slug"] = new() { Type = "string", Description = "Slug (unique)" },
                        ["price"] = new() { Type = "number", Format = "decimal", Description = "Giá" },
                        ["description"] = new() { Type = "string", Nullable = true, Description = "Mô tả ngắn" },
                        ["oldPrice"] = new() { Type = "number", Format = "decimal", Nullable = true, Description = "Giá cũ (để hiển thị giảm giá)" },
                        ["dimensionsHeight"] = new() { Type = "number", Format = "decimal", Nullable = true, Description = "Chiều cao" },
                        ["dimensionsWidth"] = new() { Type = "number", Format = "decimal", Nullable = true, Description = "Chiều rộng" },
                        ["dimensionsDepth"] = new() { Type = "number", Format = "decimal", Nullable = true, Description = "Chiều sâu" },
                        ["dimensionUnit"] = new() { Type = "string", Description = "Đơn vị kích thước (mặc định: cm)" },
                        ["detail"] = new() { Type = "string", Nullable = true, Description = "Mô tả chi tiết" },
                        ["deliveryInfo"] = new() { Type = "string", Nullable = true, Description = "Thông tin giao hàng" },
                        ["weight"] = new() { Type = "number", Format = "decimal", Nullable = true, Description = "Trọng lượng" },
                        ["isFeatured"] = new() { Type = "boolean", Description = "Sản phẩm nổi bật (mặc định: false)" },
                        ["isActive"] = new() { Type = "boolean", Description = "Trạng thái active (mặc định: true)" },
                        ["categoryIds"] = new() { Type = "string", Nullable = true, Description = "Category IDs, phân cách bằng dấu phẩy (vd: 1,2,3)" },
                        ["images"] = new() { Type = "array", Items = new OpenApiSchema { Type = "string", Format = "binary" }, Description = "File ảnh (có thể nhiều file)" }
                    },
                    required: new[] { "name", "slug", "price" })
            }
        };

        // 3. POST /api/ProductVariants/with-image
        swaggerDoc.Paths["/api/ProductVariants/with-image"] = new OpenApiPathItem
        {
            Operations = new Dictionary<OperationType, OpenApiOperation>
            {
                [OperationType.Post] = CreateMultipartOp(
                    "Tạo product variant với ảnh (multipart)",
                    "Fields: productId (required), colorName, colorCode, price, image (optional)",
                    "ProductVariants",
                    new Dictionary<string, OpenApiSchema>
                    {
                        ["productId"] = new() { Type = "integer", Description = "ID product" },
                        ["colorName"] = new() { Type = "string", Nullable = true, Description = "Tên màu" },
                        ["colorCode"] = new() { Type = "string", Nullable = true, Description = "Mã màu (#hex)" },
                        ["price"] = new() { Type = "number", Format = "decimal", Nullable = true, Description = "Giá variant" },
                        ["image"] = new() { Type = "string", Format = "binary", Nullable = true, Description = "File ảnh variant" }
                    },
                    required: new[] { "productId" })
            }
        };

        // 4. POST /api/ProductImages/with-image
        swaggerDoc.Paths["/api/ProductImages/with-image"] = new OpenApiPathItem
        {
            Operations = new Dictionary<OperationType, OpenApiOperation>
            {
                [OperationType.Post] = CreateMultipartOp(
                    "Tạo product image với upload ảnh (multipart)",
                    "Fields: productId (required), image (required), isPrimary, sortOrder, altText",
                    "ProductImages",
                    new Dictionary<string, OpenApiSchema>
                    {
                        ["productId"] = new() { Type = "integer", Description = "ID product" },
                        ["image"] = new() { Type = "string", Format = "binary", Description = "File ảnh (bắt buộc)" },
                        ["isPrimary"] = new() { Type = "boolean", Description = "Ảnh chính" },
                        ["sortOrder"] = new() { Type = "integer", Description = "Thứ tự hiển thị" },
                        ["altText"] = new() { Type = "string", Nullable = true, Description = "Alt text" }
                    },
                    required: new[] { "productId", "image" })
            }
        };

        // 5. POST /api/Upload/image
        swaggerDoc.Paths["/api/Upload/image"] = new OpenApiPathItem
        {
            Operations = new Dictionary<OperationType, OpenApiOperation>
            {
                [OperationType.Post] = new OpenApiOperation
                {
                    Summary = "Upload 1 ảnh lên Cloudinary",
                    Description = "multipart/form-data với field 'file'. Query: folder (products, categories, product-variants, product-images)",
                    Tags = new List<OpenApiTag> { new() { Name = "Upload" } },
                    Security = new List<OpenApiSecurityRequirement> { BearerAuth },
                    Parameters = new List<OpenApiParameter>
                    {
                        new() { Name = "folder", In = ParameterLocation.Query, Schema = new OpenApiSchema { Type = "string", Default = new Microsoft.OpenApi.Any.OpenApiString("products") }, Description = "products | categories | product-variants | product-images" }
                    },
                    RequestBody = new OpenApiRequestBody
                    {
                        Required = true,
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            ["multipart/form-data"] = new()
                            {
                                Schema = new OpenApiSchema
                                {
                                    Type = "object",
                                    Required = new HashSet<string> { "file" },
                                    Properties = new Dictionary<string, OpenApiSchema> { ["file"] = new() { Type = "string", Format = "binary", Description = "File ảnh" } }
                                }
                            }
                        }
                    },
                    Responses = new OpenApiResponses { ["200"] = new() { Description = "Trả về imageUrl" }, ["400"] = new() { Description = "Bad Request" }, ["401"] = new() { Description = "Unauthorized" } }
                }
            }
        };

        // 6. POST /api/Upload/images
        swaggerDoc.Paths["/api/Upload/images"] = new OpenApiPathItem
        {
            Operations = new Dictionary<OperationType, OpenApiOperation>
            {
                [OperationType.Post] = new OpenApiOperation
                {
                    Summary = "Upload nhiều ảnh lên Cloudinary",
                    Description = "multipart/form-data với field 'files' (array). Query: folder. Tối đa 10 file.",
                    Tags = new List<OpenApiTag> { new() { Name = "Upload" } },
                    Security = new List<OpenApiSecurityRequirement> { BearerAuth },
                    Parameters = new List<OpenApiParameter>
                    {
                        new() { Name = "folder", In = ParameterLocation.Query, Schema = new OpenApiSchema { Type = "string", Default = new Microsoft.OpenApi.Any.OpenApiString("products") } }
                    },
                    RequestBody = new OpenApiRequestBody
                    {
                        Required = true,
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            ["multipart/form-data"] = new()
                            {
                                Schema = new OpenApiSchema
                                {
                                    Type = "object",
                                    Required = new HashSet<string> { "files" },
                                    Properties = new Dictionary<string, OpenApiSchema> { ["files"] = new() { Type = "array", Items = new OpenApiSchema { Type = "string", Format = "binary" }, Description = "Danh sách file ảnh" } }
                                }
                            }
                        }
                    },
                    Responses = new OpenApiResponses { ["200"] = new() { Description = "Trả về imageUrls[]" }, ["400"] = new() { Description = "Bad Request" }, ["401"] = new() { Description = "Unauthorized" } }
                }
            }
        };
    }

    private static OpenApiOperation CreateMultipartOp(
        string summary,
        string description,
        string tag,
        Dictionary<string, OpenApiSchema> properties,
        string[] required)
    {
        return new OpenApiOperation
        {
            Summary = summary,
            Description = description,
            Tags = new List<OpenApiTag> { new() { Name = tag } },
            Security = new List<OpenApiSecurityRequirement> { BearerAuth },
            RequestBody = new OpenApiRequestBody
            {
                Required = true,
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Required = new HashSet<string>(required),
                            Properties = properties
                        }
                    }
                }
            },
            Responses = new OpenApiResponses
            {
                ["201"] = new OpenApiResponse { Description = "Tạo thành công" },
                ["200"] = new OpenApiResponse { Description = "Thành công" },
                ["400"] = new OpenApiResponse { Description = "Bad Request" },
                ["401"] = new OpenApiResponse { Description = "Unauthorized" }
            }
        };
    }
}
