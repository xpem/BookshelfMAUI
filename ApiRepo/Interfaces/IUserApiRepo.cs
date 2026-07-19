using Models.Responses;

namespace ApiRepo.Interfaces
{
    public interface IUserApiRepo
    {
        Task<ApiResp> AddUserAsync(string name, string email, string password);

        Task<ApiResp> RecoverPasswordAsync(string email);

        Task<ApiResp> GetTokenAsync(string email, string password);

        Task<(bool success, string? newToken)> RefreshTokenAsync(string refreshToken);

        Task<ApiResp> GetUserAsync(string token);
    }
}
