using ApiDAL.Interfaces;
using Models.Responses;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ApiDAL
{
    public class UserApiDAL : IUserApiDAL
    {
        IHttpClientFunctions HttpClientFunctions;

        public UserApiDAL(IHttpClientFunctions httpClientFunctions) { HttpClientFunctions = httpClientFunctions; }

        public async Task<ApiResponse> AddUser(string name, string email, string password)
        {
            try
            {
                string json = JsonSerializer.Serialize(new { name, email, password });

                return await HttpClientFunctions.PostAsync(ApiKeys.ApiUri + "/user", json);
            }
            catch (Exception ex) { throw ex; }
        }

        public async Task<ApiResponse> RecoverPassword(string email)
        {
            string json = JsonSerializer.Serialize(new { email });
            return await HttpClientFunctions.PostAsync(ApiKeys.ApiUri + "/user/recoverpassword", json);
        }

        public async Task<(bool, string?)> GetUserToken(string email, string password) => await HttpClientFunctions.GetUserToken(email, password);


        public async Task<ApiResponse> GetUser(string token)
        {
            try
            {
                return await HttpClientFunctions.GetAsync(ApiKeys.ApiUri + "/user", token);
            }
            catch (Exception ex) { throw ex; }
        }
    }
}
