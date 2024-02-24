using ApiDAL.Interfaces;
using Models.Responses;

namespace ApiDAL
{
    public class UserApiDAL : IUserApiDAL
    {
        private readonly UsersManagement.UserService userService = new(ApiKeys.ApiAddress);

        public async Task<ApiResponse> AddUserAsync(string name, string email, string password)
        {
            try
            {
                UsersManagement.Model.ApiResponse resp = await userService.AddUserAsync(name, email, password);

                return new() { Success = resp.Success, Content = resp.Content, Error = (ErrorTypes?)resp.Error };
            }
            catch { throw; }
        }

        public async Task<ApiResponse> RecoverPasswordAsync(string email)
        {
            UsersManagement.Model.ApiResponse resp = await userService.RecoverPasswordAsync(email);

            return new() { Success = resp.Success, Content = resp.Content, Error = (ErrorTypes?)resp.Error };
        }

        public async Task<(bool, string?)> GetUserTokenAsync(string email, string password)
        {
            UsersManagement.Model.ApiResponse resp = await userService.GetUserTokenAsync(email, password);

            return (resp.Success, resp.Content);
        }

        public async Task<ApiResponse> GetUserAsync(string token)
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
