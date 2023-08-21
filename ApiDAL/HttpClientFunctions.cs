using ApiDAL.Handlers;
using ApiDAL.Interfaces;
using LocalDbDAL.User;
using Models.Responses;
using System.Net;
using System.Text;

namespace ApiDAL
{
    public class HttpClientFunctions : HttpClient, IHttpClientFunctions
    {
        readonly IUserLocalDAL UserLocalDAL;

        public HttpClientFunctions(IUserLocalDAL userLocalDAL)
        {
            UserLocalDAL = userLocalDAL;
        }

        public async Task<bool> CheckServer()
        {
            try
            {
                HttpClient httpClient = new();

                HttpResponseMessage httpResponse = await httpClient.GetAsync(ApiKeys.ApiAddress + "/imalive");

                if (httpResponse != null && httpResponse.IsSuccessStatusCode && !string.IsNullOrEmpty(await httpResponse.Content.ReadAsStringAsync())) return true;

                return false;
            }
            catch (Exception ex) { throw ex; }// return false; }
        }

        public async Task<ApiResponse> Request(RequestsTypes requestsType, string url, string? userToken = null, string? jsonContent = null)
        {
            try
            {
                HttpClient httpClient = new();

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
                            StringContent bodyContent = new(jsonContent, Encoding.UTF8, "application/json"); ;
                            httpResponse = await httpClient.PostAsync(url, bodyContent);
                        }
                        else return new ApiResponse() { Success = false, Content = null, Error = ErrorTypes.BodyContentNull };
                        break;
                    case RequestsTypes.Put:
                        if (jsonContent is not null)
                        {
                            StringContent bodyContent = new(jsonContent, Encoding.UTF8, "application/json"); ;
                            httpResponse = await httpClient.PutAsync(url, bodyContent);
                        }
                        else return new ApiResponse() { Success = false, Content = null, Error = ErrorTypes.BodyContentNull };
                        break;
                    case RequestsTypes.Delete:
                        httpResponse = await httpClient.DeleteAsync(url);
                        break;
                }


                return new ApiResponse()
                {
                    Success = httpResponse.IsSuccessStatusCode,
                    Error = httpResponse.StatusCode == HttpStatusCode.Unauthorized ? ErrorTypes.Unauthorized : null,
                    TryRefreshToken = httpResponse.StatusCode == HttpStatusCode.Unauthorized,
                    Content = await httpResponse.Content.ReadAsStringAsync()
                };
            }
            catch (Exception ex)
            {
                if (ex.InnerException is not null && (ex.InnerException.Message == "Nenhuma conexão pôde ser feita porque a máquina de destino as recusou ativamente." || ex.InnerException.Message.Contains("Este host não é conhecido.")))
                    return new ApiResponse() { Success = false, Content = null, Error = ErrorTypes.ServerUnavaliable };

                throw ex;
            }
        }

        public async Task<ApiResponse> AuthRequest(RequestsTypes requestsType, string url, string? jsonContent = null)
        {
            bool retry = true;
            ApiResponse? resp = null;

            while (retry)
            {
                string? userToken;

                if (resp is not null && resp.TryRefreshToken)
                {
                    retry = false;

                    (bool refreshTokenSuccess, userToken) = await RefreshToken();

                    if (!refreshTokenSuccess || userToken is null)
                        return resp;
                }
                else
                {
                    userToken = await UserLocalDAL.GetUserToken();
                }

                resp = await Request(requestsType, url, userToken, jsonContent);

                if (!resp.TryRefreshToken || !retry) return resp;
            }

            throw new Exception($"Erro ao tentar AuthRequest de tipo {requestsType} na url: {url}");
        }

        private async Task<(bool success, string? newToken)> RefreshToken()
        {
            Models.User? user = await UserLocalDAL.GetUser();

            if (user is not null && user.Email is not null && user.Password is not null)
            {
                UsersManagement.IUserService userService = new UsersManagement.UserService(ApiKeys.ApiAddress);

                UsersManagement.Model.ApiResponse resp = await userService.GetUserTokenAsync(user.Email, PasswordHandler.Decrypt(user.Password));

                if (resp.Success && resp.Content is not null)
                {
                    string newToken = resp.Content;

                    await UserLocalDAL.UpdateToken(user.Id, newToken);
                    return (true, newToken);
                }
            }

            return (false, null);
        }
    }
}
