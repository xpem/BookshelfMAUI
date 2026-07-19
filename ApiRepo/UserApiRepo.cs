using ApiRepo.Interfaces;
using Models.Responses;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ApiRepo
{
    public class UserApiRepo : IUserApiRepo
    {
        private const string UserEndpoint = "/user";
        private const string SessionEndpoint = "/user/session";
        private const string RefreshSessionEndpoint = "/user/session/refresh";
        private const string RefreshTokenProperty = "refreshToken";
        private const string TokenProperty = "token";
        private const string ErrorProperty = "error";
        private const string ErrorsProperty = "errors";

        public async Task<ApiResp> AddUserAsync(string name, string email, string password)
        {
            string json = JsonSerializer.Serialize(new { name, email, password });

            return await HttpClientFunctions.Request(RequestsTypes.Post, ApiKeys.ApiAddress + UserEndpoint, jsonContent: json);
        }

        public async Task<ApiResp> RecoverPasswordAsync(string email)
        {
            string json = JsonSerializer.Serialize(new { email });

            return await HttpClientFunctions.Request(RequestsTypes.Post, ApiKeys.ApiAddress + "/user/recoverpassword", jsonContent: json);
        }

        public async Task<ApiResp> GetTokenAsync(string email, string password)
        {
            string json = JsonSerializer.Serialize(new { email, password });

            var resp = await HttpClientFunctions.Request(RequestsTypes.Post, ApiKeys.ApiAddress + SessionEndpoint, jsonContent: json);

            if (resp is null || resp.Content is null)
                throw new InvalidOperationException("Resposta inválida ao obter token.");

            if (!TryParseJson(resp.Content, out JsonNode? jResp))
                return new ApiResp { Success = false, Content = resp.Content };

            if (resp.Success)
            {
                string? token = GetStringValue(jResp, TokenProperty);

                if (!string.IsNullOrWhiteSpace(token))
                {
                    string? refreshToken = GetStringValue(jResp, RefreshTokenProperty);
                    string content = JsonSerializer.Serialize(new { token, refreshToken });

                    return new ApiResp { Success = true, Content = content };
                }
            }
            else
            {
                string? error = GetStringValue(jResp, ErrorsProperty) ?? GetStringValue(jResp, ErrorProperty);

                return new ApiResp
                {
                    Success = false,
                    Content = error ?? resp.Content
                };
            }

            return new ApiResp { Success = false, Content = resp.Content };
        }

        private static bool TryParseJson(string content, out JsonNode? node)
        {
            try
            {
                node = JsonNode.Parse(content);
                return node is not null;
            }
            catch (JsonException)
            {
                node = null;
                return false;
            }
        }

        private static string? GetStringValue(JsonNode? node, string propertyName)
        {
            JsonNode? value = node?[propertyName];

            return value switch
            {
                null => null,
                JsonValue jsonValue when jsonValue.TryGetValue<string>(out string? text) => text,
                _ => value.ToJsonString()
            };
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

        public async Task<ApiResp> GetUserAsync(string token)
        {
            return await HttpClientFunctions.Request(RequestsTypes.Get, ApiKeys.ApiAddress + UserEndpoint, userToken: token);
        }
    }
}
