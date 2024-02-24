using ApiDAL.Handlers;
using ApiDAL.Interfaces;
using DbContextDAL;
using Models.Responses;
using System.Text.Json.Nodes;

namespace BLL.User
{
    public class UserBLL(IUserApiDAL userApiDAL, IUserDAL userDAL) : IUserBLL
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

        public Task<Models.User?> GetUserLocal() => userDAL.GetUserLocal();

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

                        if (userResponse is not null)
                        {
                            Models.User? user = new()
                            {
                                Id = userResponse["id"]?.GetValue<int>() ?? 0,
                                Name = userResponse["name"]?.GetValue<string>(),
                                Email = userResponse["email"]?.GetValue<string>(),
                                Token = userTokenRes,
                                Password = PasswordHandler.Encrypt(password)
                            };

                            await userDAL.ExecuteAddUser(user);

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

        public void UpdateLocalUserLastUpdate(int uid) => userDAL.ExecuteUpdateLastUpdateUser(DateTime.Now, uid);
    }
}
