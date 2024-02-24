using Models.Responses;

namespace ApiDAL.Interfaces
{
    public interface IUserApiDAL
    {
        Task<ApiResponse> AddUserAsync(string name, string email, string password);

        Task<ApiResponse> RecoverPasswordAsync(string email);

        Task<(bool, string?)> GetUserTokenAsync(string email, string password);

        Task<ApiResponse> GetUserAsync(string token);
    }
}
