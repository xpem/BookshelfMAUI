using Models.Responses;

namespace ApiDAL.Interfaces
{
    public interface IUserApiDAL
    {
        Task<ApiResponse> AddUser(string name, string email, string password);

        Task<ApiResponse> RecoverPassword(string email);

        Task<(bool, string?)> GetUserToken(string email, string password);

        Task<ApiResponse> GetUser(string token);
    }
}
