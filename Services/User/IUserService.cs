using Models.Responses;

namespace Services.User
{
    public interface IUserService
    {
        Task<ServiceResponse> AddUser(string name, string email, string password);

        Task<string?> RecoverPassword(string email);

        Task<Models.DTOs.User?> GetUserLocal();

        Task<ServiceResponse> SignIn(string email, string password);

        void UpdateLocalUserLastUpdate(int uid);
    }
}
