using Models.Responses;

namespace ApiRepo.Interfaces
{
    public interface IUserApiRepo
    {
        Task<ApiResponse> AddUserAsync(string name, string email, string password);

        Task<ApiResponse> RecoverPasswordAsync(string email);

        Task<ApiResponse> GetTokenAsync(string email, string password);

        Task<(bool success, string? newToken)> RefreshTokenAsync(string refreshToken);

        Task<ApiResponse> GetUserAsync(string token);
    }
}
