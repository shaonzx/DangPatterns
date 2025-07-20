namespace DangPatterns.HelperModels
{
    public class ApiErrorResponse
    {
        public string CorrelationId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string Path { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public object? Details { get; set; }
    }
}
