namespace MetadataAPI.Common
{
    public class JsonValidationResult
    {
        public bool IsValid { get; }
        public List<string> Errors { get; }

        public JsonValidationResult(bool isValid, List<string> errors)
        {
            IsValid = isValid;
            Errors = errors ?? new List<string>();
        }

        public static JsonValidationResult Success() => new(true, new List<string>());

        public static JsonValidationResult Failure(IEnumerable<string> errors)
            => new(false, errors.ToList());
    }

}
