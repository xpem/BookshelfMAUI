using ApiDAL.Interfaces;
using Models.Responses;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ApiDAL
{
    public class UserApiDAL : IUserApiDAL
    {
        private const string UserEndpoint = "/user";
        private const string SessionEndpoint = "/user/session";
        private const string RefreshSessionEndpoint = "/user/session/refresh";

        public async Task<ApiResponse> AddUserAsync(string name, string email, string password)
        {
            string json = JsonSerializer.Serialize(new { name, email, password });

            return await HttpClientFunctions.Request(RequestsTypes.Post, ApiKeys.ApiAddress + UserEndpoint, jsonContent: json);
        }

        public async Task<ApiResponse> RecoverPasswordAsync(string email)
        {
            string json = JsonSerializer.Serialize(new { email });

            return await HttpClientFunctions.Request(RequestsTypes.Post, ApiKeys.ApiAddress + "/user/recoverpassword", jsonContent: json);
        }

        public async Task<ApiResponse> GetTokenAsync(string email, string password)
        {
            string json = JsonSerializer.Serialize(new { email, password });

            var resp = await HttpClientFunctions.Request(RequestsTypes.Post, ApiKeys.ApiAddress + SessionEndpoint, jsonContent: json);

            if (resp is null || resp.Content is null)
                return new ApiResponse { Success = false, Error = ErrorTypes.ServerUnavaliable };

            if (resp.Success)
            {
                JsonNode? jResp = JsonNode.Parse(resp.Content);
                string? token = jResp?["token"]?.GetValue<string>();

                if (!string.IsNullOrWhiteSpace(token))
                {
                    string? refreshToken = jResp?["refreshToken"]?.GetValue<string>();
                    string content = JsonSerializer.Serialize(new { token, refreshToken });

                    return new ApiResponse { Success = true, Content = content };
                }
            }
            else
            {
                JsonNode? jResp = JsonNode.Parse(resp.Content);
                string? error = jResp?["errors"]?.GetValue<string>() ?? jResp?["error"]?.GetValue<string>();

                return new ApiResponse
                {
                    Success = false,
                    Content = error ?? resp.Content,
                    Error = ErrorTypes.WrongEmailOrPassword
                };
            }

            return new ApiResponse { Success = false, Content = resp.Content };
        }

        public async Task<(bool success, string? newToken)> RefreshTokenAsync(string refreshToken)
        {
            string json = JsonSerializer.Serialize(new { refreshToken });

            var resp = await HttpClientFunctions.Request(RequestsTypes.Post, ApiKeys.ApiAddress + RefreshSessionEndpoint, jsonContent: json);

            if (!resp.Success || string.IsNullOrWhiteSpace(resp.Content))
                return (false, null);

            JsonNode? jResp = JsonNode.Parse(resp.Content);
            string? newToken = jResp?["token"]?.GetValue<string>();

            if (string.IsNullOrWhiteSpace(newToken))
                return (false, null);

            return (true, resp.Content);
        }

        public async Task<ApiResponse> GetUserAsync(string token)
        {
            return await HttpClientFunctions.Request(RequestsTypes.Get, ApiKeys.ApiAddress + UserEndpoint, userToken: token);
        }
    }
}
