using Models.Responses;

namespace ApiDAL.Interfaces
{
    public interface IHttpClientFunctions
    {
        Task<bool> CheckServer();

        Task<ApiResponse> Request(RequestsTypes requestsType, string url, string? userToken = null, string? jsonContent = null);

        Task<ApiResponse> AuthRequestAsync(RequestsTypes requestsType, string url, string? jsonContent = null);

    }
}
