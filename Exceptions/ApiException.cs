namespace AgroScan.API.Exceptions
{
    public class ApiException : Exception
    {
        public int StatusCode { get; }
        public string? Details { get; }

        public ApiException(int statusCode, string message, string? details = null) 
            : base(message)
        {
            StatusCode = statusCode;
            Details = details;
        }

        public ApiException(int statusCode, string message, Exception innerException, string? details = null) 
            : base(message, innerException)
        {
            StatusCode = statusCode;
            Details = details;
        }
    }
}
