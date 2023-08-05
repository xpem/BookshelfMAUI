using ApiDAL.Interfaces;
using Models.Responses;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ApiDAL
{
    public class UserApiDAL : IUserApiDAL
    {
        private readonly UsersManagement.IUserService userService = new UsersManagement.UserService(ApiKeys.ApiUri);

        public async Task<ApiResponse> AddUser(string name, string email, string password)
        {
            try
            {
               

                var resp = await userService.AddUserAsync(name, email, password);

                return new() { Success = resp.Success,Content = resp.Content,Error = (ErrorTypes?)resp.Error };
            }
            catch (Exception ex) { throw ex; }
        }

        public async Task<ApiResponse> RecoverPassword(string email)
        {
            var resp = await userService.RecoverPasswordAsync(email);

            return new() { Success = resp.Success, Content = resp.Content, Error = (ErrorTypes?)resp.Error };
        }

        public async Task<(bool, string?)> GetUserToken(string email, string password)
        {
            var resp = await userService.GetUserTokenAsync(email, password);

            return (resp.Success, resp.Content);
        }

        public async Task<ApiResponse> GetUser(string token)
        {
            try
            {
                var resp = await userService.GetUserAsync(token);

                return new() { Success = resp.Success, Content = resp.Content, Error = (ErrorTypes?)resp.Error };
            }
            catch (Exception ex) { throw ex; }
        }
    }
}
