using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace HappyFurnitureBE.API.Filters;

public class FileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasFileParameter = context.MethodInfo.GetParameters()
            .Any(p => p.ParameterType == typeof(IFormFile) || 
                     (p.ParameterType.IsGenericType && p.ParameterType.GetGenericTypeDefinition() == typeof(Nullable<>) && Nullable.GetUnderlyingType(p.ParameterType) == typeof(IFormFile)) ||
                     p.ParameterType == typeof(List<IFormFile>) ||
                     p.ParameterType == typeof(IFormFileCollection));

        if (!hasFileParameter) return;

        // Set request body to multipart/form-data
        operation.RequestBody = new OpenApiRequestBody
        {
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["multipart/form-data"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        Properties = new Dictionary<string, OpenApiSchema>(),
                        Required = new HashSet<string>()
                    }
                }
            }
        };

        var formDataSchema = operation.RequestBody.Content["multipart/form-data"].Schema;

        // Add parameters to schema
        foreach (var parameter in context.MethodInfo.GetParameters())
        {
            var fromFormAttribute = parameter.GetCustomAttribute<FromFormAttribute>();
            if (fromFormAttribute == null) continue;

            var propertyName = parameter.Name!;

            if (parameter.ParameterType == typeof(IFormFile) || 
                (parameter.ParameterType.IsGenericType && parameter.ParameterType.GetGenericTypeDefinition() == typeof(Nullable<>) && Nullable.GetUnderlyingType(parameter.ParameterType) == typeof(IFormFile)))
            {
                formDataSchema.Properties[propertyName] = new OpenApiSchema
                {
                    Type = "string",
                    Format = "binary"
                };
            }
            else if (parameter.ParameterType == typeof(List<IFormFile>) || parameter.ParameterType == typeof(IFormFileCollection))
            {
                formDataSchema.Properties[propertyName] = new OpenApiSchema
                {
                    Type = "array",
                    Items = new OpenApiSchema
                    {
                        Type = "string",
                        Format = "binary"
                    }
                };
            }
            else
            {
                // Handle other form parameters
                var schema = new OpenApiSchema();
                
                if (parameter.ParameterType == typeof(string))
                {
                    schema.Type = "string";
                }
                else if (parameter.ParameterType == typeof(int) || 
                         (parameter.ParameterType.IsGenericType && parameter.ParameterType.GetGenericTypeDefinition() == typeof(Nullable<>) && Nullable.GetUnderlyingType(parameter.ParameterType) == typeof(int)))
                {
                    schema.Type = "integer";
                    schema.Format = "int32";
                }
                else if (parameter.ParameterType == typeof(decimal) || 
                         (parameter.ParameterType.IsGenericType && parameter.ParameterType.GetGenericTypeDefinition() == typeof(Nullable<>) && Nullable.GetUnderlyingType(parameter.ParameterType) == typeof(decimal)))
                {
                    schema.Type = "number";
                    schema.Format = "decimal";
                }
                else if (parameter.ParameterType == typeof(bool) || 
                         (parameter.ParameterType.IsGenericType && parameter.ParameterType.GetGenericTypeDefinition() == typeof(Nullable<>) && Nullable.GetUnderlyingType(parameter.ParameterType) == typeof(bool)))
                {
                    schema.Type = "boolean";
                }
                else
                {
                    schema.Type = "string";
                }

                formDataSchema.Properties[propertyName] = schema;
            }

            // Mark as required if parameter is not nullable
            if (!IsNullable(parameter.ParameterType))
            {
                formDataSchema.Required.Add(propertyName);
            }
        }

        // Clear parameters since we're using requestBody
        operation.Parameters.Clear();
    }

    private static bool IsNullable(Type type)
    {
        return !type.IsValueType || 
               (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
    }
}