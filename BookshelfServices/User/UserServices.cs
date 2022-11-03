using BookshelfModels.Books;
using BookshelfRepos.User;
using BookshelfServices.User.AuthServices;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace BookshelfServices.User
{
    public class UserServices : IUserServices
    {
        private readonly IUserAuthServices userAuthServices;
        static HttpClient httpClient = new();

        public UserServices(IUserAuthServices _userAuthServices)
        {
            userAuthServices = _userAuthServices;
        }

        /// <summary>
        /// get user in the static var
        /// </summary>
        /// <returns></returns>
        public BookshelfModels.User.User? GetUserLocal() => UserRepos.GetUser();

        public async Task<BookshelfModels.User.User?> RefreshUserToken(BookshelfModels.User.User? user)
        {
            (BookshelfModels.User.User? userResponse, bool Success) = await userAuthServices.RefreshUserToken(user);

            if (Success && userResponse is not null)
            {
                UserRepos.UpdateToken(userResponse.Id, userResponse.Token);
            }

            return userResponse;
        }

        /// <summary>
        /// Get user and add in sqlite
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> SignIn(string email, string password)
        {
            try
            {
                int forContinue = 0;

                while (forContinue < 2)
                {
                    string json = JsonSerializer.Serialize(new { email, password });
                    StringContent data = new(json, Encoding.UTF8, "application/json");

                    //to do - make a base to this
                    httpClient = new HttpClient();
                    HttpResponseMessage response = await httpClient.PostAsync(ApiKeys.ApiUri + "/user/signin", data);
                    string result = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode)
                    {
                        JsonNode? obj = JsonNode.Parse(result);

                        if (obj != null)
                        {
                            string? token = obj["token"]?.GetValue<string>();

                            if (token != null)
                            {
                                httpClient = new HttpClient();
                                httpClient.DefaultRequestHeaders.Add("authorization", "bearer " + token);
                                response = await httpClient.GetAsync(ApiKeys.ApiUri + "/user");
                                result = response.Content.ReadAsStringAsync().Result;

                                if (response.IsSuccessStatusCode)
                                {
                                    obj = JsonNode.Parse(result);

                                    if (obj != null)
                                    {
                                        BookshelfModels.User.User UserResponse = new() { Id = obj["id"]?.GetValue<int>().ToString(), Name = obj["name"]?.GetValue<string>(), Email = obj["email"]?.GetValue<string>() };

                                        if (UserResponse is not null)
                                        {
                                            if (UserResponse.Error != null)
                                                return false;
                                            else
                                            {
                                                UserRepos.InsertUser(UserResponse);
                                                return true;
                                            }
                                        }
                                        else
                                            return false;
                                    }
                                }
                                else throw new Exception(result);
                            }
                            else throw new Exception(result);
                        }
                        else throw new Exception(result);
                    }
                }

                throw new Exception("O servidor está indisponível");
            }
            catch (Exception) { throw; }
        }

        public async Task<BookshelfModels.User.User> InsertUser(string name, string email, string password)
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

        public static void CleanUserDatabase() => UserRepos.CleanUserDatabase();
    }
}
