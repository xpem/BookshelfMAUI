using Models.Responses;

namespace BLL.User
{
    public interface IUserService
    {
        Task<BLLResponse> AddUser(string name, string email, string password);

        Task<string?> RecoverPassword(string email);

        Task<(bool, string?)> GetUserToken(string email, string password);

        Task<Models.User?> GetUserLocal();

        Task<BLLResponse> SignIn(string email, string password);

        void UpdateLocalUserLastUpdate(int uid);
    }
}
