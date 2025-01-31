using MetadataAPI.Common;
using MetadataAPI.Infrastructure.Interfaces;
using NJsonSchema;
using System.Reflection;

namespace MetadataAPI.Infrastructure.Services
{
    public class JsonSchemaValidator : IJsonSchemaValidator
    {
        private const string SchemaFile = "MetadataAPI.Resources.ClinicalTrialSchema.json";

        public async Task<JsonValidationResult> ValidateJsonAsync(string jsonData)
        {
            var schema = await LoadSchemaAsync();
            var json = Newtonsoft.Json.Linq.JObject.Parse(jsonData);
            var errors = schema.Validate(json);

            if (errors.Count > 0)
            {
                // Handle validation errors, maybe log them
                return JsonValidationResult.Failure(errors.Select(e => e.ToString()).ToList());
            }

            return JsonValidationResult.Success();
        }

        private async Task<JsonSchema> LoadSchemaAsync()
        {
            var assembly = Assembly.GetExecutingAssembly();
            await using var stream = assembly.GetManifestResourceStream(SchemaFile);
            using var reader = new StreamReader(stream);
            var schemaJson = await reader.ReadToEndAsync();

            return await JsonSchema.FromJsonAsync(schemaJson);
        }
    }
}
