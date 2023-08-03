using Models.Responses;

namespace ApiDAL.Interfaces
{
    public interface IHttpClientFunctions
    {
        Task<bool> CheckServer();

        Task<ApiResponse> Request(RequestsTypes requestsType, string url, string? userToken = null, string? jsonContent = null);

        Task<ApiResponse> AuthRequest(RequestsTypes requestsType, string url, string? jsonContent = null);

        Task<ApiResponse> GetAsync(string uri, string userToken);

        Task<ApiResponse> PostAsync(string uri, string jsonContent, string? userToken = null);

        Task<ApiResponse> PutAsync(string uri, string jsonContent, string? userToken = null);

        Task<ApiResponse> DeleteAsync(string uri, string? userToken = null);

        Task<(bool, string?)> GetUserToken(string email, string password);

    }
}
