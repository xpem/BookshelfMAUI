using ApiDAL;
using ApiDAL.Handlers;
using LocalDbDAL.BuildDb;
using LocalDbDAL.User;
using Models;
using Models.Responses;
using System.Text.Json.Nodes;

namespace BLL.User
{
    public static class UserBLL //: IUserServices
    {
        public static async Task<BLLResponse> AddUser(string name, string email, string password)
        {
            email = email.ToLower();
            ApiResponse? resp = await UserApiDAL.AddUser(name, email, password);

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

        public static async Task<string?> RecoverPassword(string email)
        {
            email = email.ToLower();
            ApiResponse? resp = await UserApiDAL.RecoverPassword(email);

            if (resp is not null && resp.Content is not null)
            {
                JsonNode? jResp = JsonNode.Parse(resp.Content);
                if (jResp is not null)
                {
                    return jResp["Mensagem"]?.GetValue<string>();
                }
            }

            return null;
        }

        public static async Task<(bool, string?)> GetUserToken(string email, string password)
        {
            try
            {
                email = email.ToLower();

                return await UserApiDAL.GetUserToken(email, password);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// get user in the static var
        /// </summary>
        /// <returns></returns>
        public static async Task<Models.User?> GetUserLocal() => await UserLocalDAL.GetUser();

        //public async Task<Models.User?> RefreshUserToken(Models.User user)
        //{
        //    try
        //    {
        //        Models.User userResponse = user;

        //        if (!string.IsNullOrEmpty(user.Email) && !string.IsNullOrEmpty(user.Password))
        //        {
        //            (bool success, string? res) = await UserApiService.GetUserToken(user.Email, user.Password);

        //            if (success && res != null)
        //            {
        //                await UserRepos.UpdateToken(user.Id, res);
        //                userResponse.Token = res;
        //            }
        //            else
        //            {
        //                if (res != null)
        //                    userResponse.Error = ErrorType.WrongEmailOrPassword;
        //            }
        //            return userResponse;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    catch (Exception ex) { throw ex; }
        //}

        public static async Task<BLLResponse> GetUser(string email, string password)
        {
            try
            {
                email = email.ToLower();

                (bool success, string? userTokenRes) = await GetUserToken(email, password);

                if (success && userTokenRes != null)
                {
                    ApiResponse resp = await UserApiDAL.GetUser(userTokenRes);

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

                            UserLocalDAL.InsertUser(user);

                            return new BLLResponse() { Success = true };
                        }
                    }
                }
                //maybe use a errorcodes instead a message?
                else if (!success && userTokenRes is not null && userTokenRes == "User/Password incorrect")
                    return new BLLResponse() { Success = false, Error = ErrorTypes.WrongEmailOrPassword };
                else throw new Exception("Erro não mapeado");

                return new BLLResponse() { Success = false, Error = ErrorTypes.Unknown };
            }
            catch (Exception ex) { throw ex; }
        }

        public static Task CleanDatabase() => BuildLocalDbDAL.CleanDatabase();

        //public async Task<Models.User> SignUp(string name, string email, string password) => await UserApiService.SignUp(name, email, password);
    }
}
