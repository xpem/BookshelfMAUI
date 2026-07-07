using ApiRepo.Interfaces;
using Models.Responses;
using Repos.Interfaces;
using System.Text.Json.Nodes;

namespace Services.User
{
    public class UserService(IUserApiRepo UserApiRepo, IUserRepo userRepo, IBuildDbService BuildDbService) : IUserService
    {
        public async Task<ServiceResponse> AddUser(string name, string email, string password)
        {
            email = email.ToLower();
            ApiResponse? resp = await UserApiRepo.AddUserAsync(name, email, password);

            if (resp is not null && resp.Success && resp.Content is not null)
            {
                JsonNode? jResp = JsonNode.Parse(resp.Content);
                if (jResp is not null)
                {
                    Models.DTOs.User user = new()
                    {
                        Id = jResp["id"]?.GetValue<int>() ?? 0,
                        Name = jResp["name"]?.GetValue<string>(),
                        Email = jResp["email"]?.GetValue<string>()
                    };

                    if (user.Id is not 0)
                        return new ServiceResponse() { Success = resp.Success };
                }
            }

            return new ServiceResponse() { Success = false, Content = null };
        }

        public async Task<string?> RecoverPassword(string email)
        {
            email = email.ToLower();
            ApiResponse? resp = await UserApiRepo.RecoverPasswordAsync(email);

            if (resp is not null && resp.Content is not null)
            {
                JsonNode? jResp = JsonNode.Parse(resp.Content);
                if (jResp is not null)
                    return jResp["Mensagem"]?.GetValue<string>();
            }

            return null;
        }

        public Task<Models.DTOs.User?> GetUserLocal() => userRepo.GetUserLocalAsync();

        public async Task<ServiceResponse> SignIn(string email, string password)
        {
            try
            {
                email = email.ToLower();

                ApiResponse tokenResp = await UserApiRepo.GetTokenAsync(email, password);

                if (tokenResp.Success && tokenResp.Content is not null)
                {
                    JsonNode? tokenJson = JsonNode.Parse(tokenResp.Content);
                    string? userToken = tokenJson?["token"]?.GetValue<string>();
                    string? refreshToken = tokenJson?["refreshToken"]?.GetValue<string>();

                    if (userToken is not null)
                    {
                        ApiResponse resp = await UserApiRepo.GetUserAsync(userToken);

                        if (resp.Success && resp.Content != null)
                        {
                            JsonNode? userResponse = JsonNode.Parse(resp.Content);
                            Models.DTOs.User? user;

                            if (userResponse is not null)
                            {
                                user = new()
                                {
                                    Id = userResponse["id"]?.GetValue<int>() ?? 0,
                                    Name = userResponse["name"]?.GetValue<string>(),
                                    Email = userResponse["email"]?.GetValue<string>(),
                                    Token = userToken,
                                    RefreshToken = refreshToken
                                };

                                Models.DTOs.User? actualUser = await userRepo.GetUserLocalAsync();

                                if (actualUser != null)
                                {
                                    if (actualUser.Id == user.Id)
                                        await userRepo.UpdateAsync(user);
                                    else
                                    {
                                        await BuildDbService.CleanLocalDatabase();
                                        await userRepo.CreateAsync(user);
                                    }
                                }
                                else
                                    await userRepo.CreateAsync(user);

                                return new ServiceResponse() { Success = true, Content = user.Id };
                            }
                        }
                    }
                }
                else if (!tokenResp.Success && tokenResp.Error == ErrorTypes.WrongEmailOrPassword)
                    return new ServiceResponse() { Success = false, Error = ErrorTypes.WrongEmailOrPassword };
                else
                    return new ServiceResponse() { Success = false, Error = ErrorTypes.ServerUnavaliable };

                return new ServiceResponse() { Success = false, Error = ErrorTypes.Unknown };
            }
            catch (Exception ex) { throw; }
        }

        public void UpdateLocalUserLastUpdate(int uid)
        {
            userRepo.UpdateLastUpdate(DateTime.Now, uid);
        }
    }
}
