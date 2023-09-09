using ApiDAL.Handlers;
using ApiDAL.Interfaces;
using DBContextDAL;
using Models.Responses;
using System.Text.Json.Nodes;

namespace BLL.User
{
    public class UserBLL : IUserBLL
    {
        readonly IUserApiDAL UserApiDAL;
        private readonly BookshelfDbContext bookshelfDbContext;

        public UserBLL(IUserApiDAL userApiDAL, BookshelfDbContext bookshelfDBContext)
        {
            UserApiDAL = userApiDAL;
            this.bookshelfDbContext = bookshelfDBContext;
        }

        public async Task<BLLResponse> AddUser(string name, string email, string password)
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

        public async Task<string?> RecoverPassword(string email)
        {
            email = email.ToLower();
            ApiResponse? resp = await UserApiDAL.RecoverPassword(email);

            if (resp is not null && resp.Content is not null)
            {
                JsonNode? jResp = JsonNode.Parse(resp.Content);
                if (jResp is not null)
                    return jResp["Mensagem"]?.GetValue<string>();
            }

            return null;
        }

        public async Task<(bool, string?)> GetUserToken(string email, string password)
        {
            try
            {
                return await UserApiDAL.GetUserToken(email.ToLower(), password);
            }
            catch { throw; }
        }

        public Task<Models.User?> GetUserLocal() => Task.Run(() => bookshelfDbContext.User.FirstOrDefault());

        public Task<int> GetUid() => Task.Run(() => bookshelfDbContext.User.Select(x => x.Id).First());

        public async Task<BLLResponse> SignIn(string email, string password)
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

                            bookshelfDbContext.ChangeTracker.Clear();

                            await bookshelfDbContext.User.AddAsync(user);
                            await bookshelfDbContext.SaveChangesAsync();

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

        public async Task UpdateLocalUserLastUpdate(Models.User user)
        {
            user.LastUpdate = DateTime.Now;

            bookshelfDbContext.ChangeTracker?.Clear();

            bookshelfDbContext.Update(user);
            await bookshelfDbContext.SaveChangesAsync();
        }
    }
}
