using ApiDAL.Handlers;
using ApiDAL.Interfaces;
using Repositories.Interfaces;
using Models.Responses;
using System.Text.Json.Nodes;

namespace BLL.User
{
    public class UserService(IUserApiDAL userApiDAL, IUserRepo userRepo, IBuildDbBLL buildDbBLL) : IUserService
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
                    Models.User user = new()
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

        public async Task<(bool, string?)> GetUserToken(string email, string password) => await userApiDAL.GetUserTokenAsync(email.ToLower(), password);

        public Task<Models.User?> GetUserLocal() => userRepo.GetUserLocalAsync();

        public async Task<BLLResponse> SignIn(string email, string password)
        {
            try
            {
                email = email.ToLower();

                (bool success, string? userTokenRes) = await GetUserToken(email, password);

                if (success && userTokenRes != null)
                {
                    ApiResponse resp = await userApiDAL.GetUserAsync(userTokenRes);

                    if (resp.Success && resp.Content != null)
                    {
                        JsonNode? userResponse = JsonNode.Parse(resp.Content);
                        Models.User? user;

                        if (userResponse is not null)
                        {
                            user = new()
                            {
                                Id = userResponse["id"]?.GetValue<int>() ?? 0,
                                Name = userResponse["name"]?.GetValue<string>(),
                                Email = userResponse["email"]?.GetValue<string>(),
                                Token = userTokenRes,
                                Password = EncryptionService.Encrypt(password)
                            };

                            Models.User? actualUser = await userRepo.GetUserLocalAsync();

                            //resign 
                            if (actualUser != null)
                            {
                                //with the same user
                                if ((actualUser.Id == user.Id))
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
                //maybe use a errorcodes instead a message?
                else if (!success && userTokenRes is not null && (userTokenRes is "User/Password incorrect" or "Invalid Email"))
                    return new BLLResponse() { Success = false, Error = ErrorTypes.WrongEmailOrPassword };
                else return new BLLResponse() { Success = false, Error = ErrorTypes.ServerUnavaliable }; ;

                return new BLLResponse() { Success = false, Error = ErrorTypes.Unknown };
            }
            catch (Exception ex) { throw ex; }
        }

        public void UpdateLocalUserLastUpdate(int uid) => userRepo.UpdateLastUpdate(DateTime.Now, uid);
    }
}
