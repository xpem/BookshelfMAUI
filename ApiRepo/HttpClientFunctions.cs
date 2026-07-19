using ApiRepo.Interfaces;
using Models.DTOs;
using Models.Responses;
using Repos;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ApiRepo
{
    public class HttpClientFunctions(BookshelfDbContext bookshelfDbContext) : HttpClient, IHttpClientFunctions
    {
        private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(20);

        public async Task<bool> CheckServer()
        {
            try
            {
                HttpClient httpClient = new() { Timeout = RequestTimeout };

                HttpResponseMessage httpResponse = await httpClient.GetAsync(ApiKeys.ApiAddress + "/imalive");

                if (httpResponse != null && httpResponse.IsSuccessStatusCode && !string.IsNullOrEmpty(await httpResponse.Content.ReadAsStringAsync())) return true;

                return false;
            }
            catch (Exception ex) { throw ex; }
        }

        public static async Task<ApiResp> Request(RequestsTypes requestsType, string url, string? userToken = null, string? jsonContent = null)
        {
            try
            {
                HttpClient httpClient = new(new HttpClientHandler
                {
                    SslProtocols = System.Security.Authentication.SslProtocols.Tls12
                })
                {
                    Timeout = RequestTimeout
                };

                if (userToken is not null)
                    httpClient.DefaultRequestHeaders.Add("authorization", "bearer " + userToken);

                HttpResponseMessage httpResponse = new();

                switch (requestsType)
                {
                    case RequestsTypes.Get:
                        httpResponse = await httpClient.GetAsync(url);
                        break;
                    case RequestsTypes.Post:
                        if (jsonContent is not null)
                        {
                            StringContent bodyContent = new(jsonContent, Encoding.UTF8, "application/json");
                            httpResponse = await httpClient.PostAsync(url, bodyContent);
                        }
                        else return new ApiResp() { Success = false, Content = null, ErrorCode = ErrorCodeTypes.Unknown };
                        break;
                    case RequestsTypes.Put:
                        if (jsonContent is not null)
                        {
                            StringContent bodyContent = new(jsonContent, Encoding.UTF8, "application/json");
                            httpResponse = await httpClient.PutAsync(url, bodyContent);
                        }
                        else return new ApiResp() { Success = false, Content = null, ErrorCode = ErrorCodeTypes.Unknown };
                        break;
                    case RequestsTypes.Delete:
                        httpResponse = await httpClient.DeleteAsync(url);
                        break;
                }

                return new ApiResp()
                {
                    Success = httpResponse.IsSuccessStatusCode,
                    ErrorCode = httpResponse.StatusCode == HttpStatusCode.Unauthorized ? ErrorCodeTypes.Unauthorized : ErrorCodeTypes.Unknown,
                    TryRefreshToken = httpResponse.StatusCode == HttpStatusCode.Unauthorized,
                    Content = await httpResponse.Content.ReadAsStringAsync()
                };
            }
            catch (Exception ex)
            {
                if (ex.InnerException is not null && (ex.InnerException.Message == "Nenhuma conexão pôde ser feita porque a máquina de destino as recusou ativamente." || ex.InnerException.Message.Contains("Este host não é conhecido.")))
                    return new ApiResp() { Success = false, Content = null, ErrorCode = ErrorCodeTypes.ServerUnavaliable };

                throw ex;
            }
        }

        Task<ApiResp> IHttpClientFunctions.Request(RequestsTypes requestsType, string url, string? userToken, string? jsonContent)
            => Request(requestsType, url, userToken, jsonContent);

        public async Task<ApiResp> AuthRequestAsync(RequestsTypes requestsType, string url, string? jsonContent = null)
        {
            UserDTO? user = bookshelfDbContext.User.FirstOrDefault();
            string? userToken = user?.Token;

            if (userToken is null) throw new ArgumentNullException(nameof(userToken));

            ApiResp resp = await Request(requestsType, url, userToken, jsonContent);

            if (!resp.TryRefreshToken)
                return resp;

            (bool refreshTokenSuccess, string? newToken) = await RefreshToken();

            if (!refreshTokenSuccess || string.IsNullOrWhiteSpace(newToken))
                return resp;

            return await Request(requestsType, url, newToken, jsonContent);
        }

        private async Task<(bool success, string? newToken)> RefreshToken()
        {
            UserDTO? user = bookshelfDbContext.User.FirstOrDefault();

            if (user is null || string.IsNullOrWhiteSpace(user.RefreshToken))
                return (false, null);

            string json = JsonSerializer.Serialize(new { refreshToken = user.RefreshToken });

            ApiResp resp = await Request(RequestsTypes.Post, ApiKeys.ApiAddress + "/user/session/refresh", jsonContent: json);

            if (!resp.Success || string.IsNullOrWhiteSpace(resp.Content))
                return (false, null);

            JsonNode? jResp = JsonNode.Parse(resp.Content);
            string? newToken = jResp?["token"]?.GetValue<string>();

            if (string.IsNullOrWhiteSpace(newToken))
                return (false, null);

            string? newRefreshToken = jResp?["refreshToken"]?.GetValue<string>();

            user.Token = newToken;
            user.RefreshToken = newRefreshToken;

            bookshelfDbContext.Update(user);
            await bookshelfDbContext.SaveChangesAsync();

            return (true, newToken);
        }
    }
}
