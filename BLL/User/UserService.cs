using ApiDAL.Interfaces;
using Models.Responses;
using Repos.Interfaces;
using System.Text.Json.Nodes;

namespace Services.User
{
    public class UserService(IUserApiDAL userApiDAL, IUserRepo userRepo, IBuildDbService buildDbBLL) : IUserService
    {
        public async Task<BLLResponse> AddUser(string name, string email, string password)
        {
            email = email.ToLower();
            ApiResponse? resp = await userApiDAL.AddUserAsync(name, email, password);

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
                        return new BLLResponse() { Success = resp.Success };
                }
            }

            return new BLLResponse() { Success = false, Content = null };
        }

        public async Task<string?> RecoverPassword(string email)
        {
            email = email.ToLower();
            ApiResponse? resp = await userApiDAL.RecoverPasswordAsync(email);

            if (resp is not null && resp.Content is not null)
            {
                JsonNode? jResp = JsonNode.Parse(resp.Content);
                if (jResp is not null)
                    return jResp["Mensagem"]?.GetValue<string>();
            }

            return null;
        }

        public Task<Models.DTOs.User?> GetUserLocal() => userRepo.GetUserLocalAsync();

        public async Task<BLLResponse> SignIn(string email, string password)
        {
            try
            {
                email = email.ToLower();

                ApiResponse tokenResp = await userApiDAL.GetTokenAsync(email, password);

                if (tokenResp.Success && tokenResp.Content is not null)
                {
                    JsonNode? tokenJson = JsonNode.Parse(tokenResp.Content);
                    string? userToken = tokenJson?["token"]?.GetValue<string>();
                    string? refreshToken = tokenJson?["refreshToken"]?.GetValue<string>();

                    if (userToken is not null)
                    {
                        ApiResponse resp = await userApiDAL.GetUserAsync(userToken);

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
                                        await buildDbBLL.CleanLocalDatabase();
                                        await userRepo.CreateAsync(user);
                                    }
                                }
                                else
                                    await userRepo.CreateAsync(user);

                                return new BLLResponse() { Success = true, Content = user.Id };
                            }
                        }
                    }
                }
                else if (!tokenResp.Success && tokenResp.Error == ErrorTypes.WrongEmailOrPassword)
                    return new BLLResponse() { Success = false, Error = ErrorTypes.WrongEmailOrPassword };
                else
                    return new BLLResponse() { Success = false, Error = ErrorTypes.ServerUnavaliable };

                return new BLLResponse() { Success = false, Error = ErrorTypes.Unknown };
            }
            catch (Exception ex) { throw; }
        }

        public void UpdateLocalUserLastUpdate(int uid)
        {
            userRepo.UpdateLastUpdate(DateTime.Now, uid);
        }
    }
}
