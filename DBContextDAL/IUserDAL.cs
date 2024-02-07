using Models;

namespace DbContextDAL
{
    public interface IUserDAL
    {
        Task<int> ExecuteAddUser(User user);
        int ExecuteUpdateLastUpdateUser(DateTime lastUpdate, int uid);
        int GetUid();
        Task<User?> GetUserLocal();
    }
}