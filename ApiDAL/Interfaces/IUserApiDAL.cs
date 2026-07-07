using Models.Responses;

namespace ApiDAL.Interfaces
{
    public interface IUserApiDAL
    {
        Task<ApiResponse> AddUserAsync(string name, string email, string password);

        Task<ApiResponse> RecoverPasswordAsync(string email);

        Task<ApiResponse> GetTokenAsync(string email, string password);

        Task<(bool success, string? newToken)> RefreshTokenAsync(string refreshToken);

        Task<ApiResponse> GetUserAsync(string token);
    }
}
