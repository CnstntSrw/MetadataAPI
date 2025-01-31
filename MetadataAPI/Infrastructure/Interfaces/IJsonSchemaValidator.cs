using MetadataAPI.Common;

namespace MetadataAPI.Infrastructure.Interfaces
{
    public interface IJsonSchemaValidator
    {
        Task<JsonValidationResult> ValidateJsonAsync(string jsonData);
    }
}
