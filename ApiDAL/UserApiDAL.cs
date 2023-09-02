using ApiDAL.Interfaces;
using Models.Responses;

namespace ApiDAL
{
    public class UserApiDAL : IUserApiDAL
    {
        private readonly UsersManagement.IUserService userService = new UsersManagement.UserService(ApiKeys.ApiAddress);

        public async Task<ApiResponse> AddUser(string name, string email, string password)
        {
            try
            {
                UsersManagement.Model.ApiResponse resp = await userService.AddUserAsync(name, email, password);

                return new() { Success = resp.Success, Content = resp.Content, Error = (ErrorTypes?)resp.Error };
            }
            catch { throw; }
        }

        public async Task<ApiResponse> RecoverPassword(string email)
        {
            UsersManagement.Model.ApiResponse resp = await userService.RecoverPasswordAsync(email);

            return new() { Success = resp.Success, Content = resp.Content, Error = (ErrorTypes?)resp.Error };
        }

        public async Task<(bool, string?)> GetUserToken(string email, string password)
        {
            UsersManagement.Model.ApiResponse resp = await userService.GetUserTokenAsync(email, password);

            return (resp.Success, resp.Content);
        }

        public async Task<ApiResponse> GetUser(string token)
        {
            try
            {
                UsersManagement.Model.ApiResponse resp = await userService.GetUserAsync(token);

                return new() { Success = resp.Success, Content = resp.Content, Error = (ErrorTypes?)resp.Error };
            }
            catch { throw; }
        }
    }
}
