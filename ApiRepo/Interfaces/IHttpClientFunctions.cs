using Models.Responses;

namespace ApiRepo.Interfaces
{
    public interface IHttpClientFunctions
    {
        Task<bool> CheckServer();

        Task<ApiResp> Request(RequestsTypes requestsType, string url, string? userToken = null, string? jsonContent = null);

        Task<ApiResp> AuthRequestAsync(RequestsTypes requestsType, string url, string? jsonContent = null);

    }
}
