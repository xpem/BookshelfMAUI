using BookshelfModels.User;
using BookshelfRepos.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace BookshelfServices.User.Api
{
    public static class UserApiService
    {
        static HttpClient httpClient = new();

        public static async Task<(bool, string?)> GetUserToken(string email, string password)
        {
            try
            {
                string json = JsonSerializer.Serialize(new { email, password });
                StringContent data = new(json, Encoding.UTF8, "application/json");

                //to do - make a base to this
                httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.PostAsync(ApiKeys.ApiUri + "/user/session", data);
                string result = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    JsonNode? obj = JsonNode.Parse(result);

                    if (obj != null)
                    {
                        string? token = obj["token"]?.GetValue<string>();

                        if (token != null)
                        {
                            return (true, token);

                        }
                        else throw new Exception(result);
                    }
                    else throw new Exception(result);
                }
                else
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        if (result != null)
                            return (false, result);
                    }
                }

                return (false, "Favor logar novamente");
            }
            catch (Exception) { throw; }
        }


        public static async Task<BookshelfModels.User.User> GetUser(string email, string password)
        {
            try
            {
                BookshelfModels.User.User? user = new();

                (bool success, string? userTokenRes) = await GetUserToken(email, password);

                if (success && userTokenRes != null)
                {
                    httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Add("authorization", "bearer " + userTokenRes);
                    var response = await httpClient.GetAsync(ApiKeys.ApiUri + "/user");
                    var result = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var obj = JsonNode.Parse(result);

                        if (obj != null)
                        {
                            user = new() { Id = obj["id"]?.GetValue<int>().ToString(), Name = obj["name"]?.GetValue<string>(), Email = obj["email"]?.GetValue<string>(), Token = userTokenRes, Password = password };
                        }
                        else throw new Exception(result);
                    }
                    else throw new Exception(result);
                }
                else user.Error = ErrorType.WrongEmailOrPassword; ;

                return user;
            }
            catch (Exception) { throw; }
        }

        public static async Task<BookshelfModels.User.User> AddUser(string name, string email, string password)
        {
            try
            {
                int forContinue = 0;

                while (forContinue < 2)
                {
                    string json = JsonSerializer.Serialize(new { name, email, password });
                    StringContent data = new(json, Encoding.UTF8, "application/json");

                    //to do - make a base to this
                    httpClient = new HttpClient();
                    HttpResponseMessage response = await httpClient.PostAsync(ApiKeys.ApiUri + "/user", data);
                    string result = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode)
                    {
                        JsonNode? obj = JsonNode.Parse(result);

                        if (obj != null)
                        {
                            BookshelfModels.User.User userResponse = new() { Id = obj["id"]?.GetValue<int>().ToString(), Name = obj["name"]?.GetValue<string>(), Email = obj["email"]?.GetValue<string>() };

                            return (userResponse);
                        }
                        else throw new Exception(result);
                    }
                }

                throw new Exception("O servidor está indisponível");
            }
            catch (Exception) { throw; }
        }

    }
}
