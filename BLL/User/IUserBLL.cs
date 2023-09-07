using Models.Responses;

namespace BLL.User
{
    public interface IUserBLL
    {
        Task<BLLResponse> AddUser(string name, string email, string password);

        Task<string?> RecoverPassword(string email);

        Task<(bool, string?)> GetUserToken(string email, string password);

        Task<Models.User?> GetUserLocal();

        Task<int> GetUid();

        Task<BLLResponse> SignIn(string email, string password);

        Task UpdateLocalUserLastUpdate(Models.User user);
    }
}
