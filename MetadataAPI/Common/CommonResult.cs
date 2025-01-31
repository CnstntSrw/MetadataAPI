namespace MetadataAPI.Common
{
    public class CommonResult
    {
        public bool IsSuccess { get; }
        public string? ErrorMessage { get; }

        private CommonResult(bool success, string? errorMessage)
        {
            IsSuccess = success;
            ErrorMessage = errorMessage;
        }

        public static CommonResult Success() => new(true, null);
        public static CommonResult Failure(string errorMessage) => new(false, errorMessage);
    }

}
