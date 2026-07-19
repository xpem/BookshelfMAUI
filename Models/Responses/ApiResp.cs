using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Models.Responses
{
    public class ApiResp
    {
        public bool Success { get; set; }

        public string? Content { get; set; }

        public ApiError? Error { get; init; }

        public ErrorCodeTypes ErrorCode { get; init; }

        public bool TryRefreshToken { get; set; }
    }

    public record ApiError
    {
        public required string Message { get; init; }
    }
}
