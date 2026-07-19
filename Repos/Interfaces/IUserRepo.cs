using Models.DTOs;

namespace Repos.Interfaces
{
    public interface IUserRepo
    {
        Task<int> CreateAsync(UserDTO user);

        Task UpdateAsync(UserDTO user);

        int UpdateLastUpdate(DateTime lastUpdate, int uid);
        //Task<int> GetUidAsync();
        Task<UserDTO?> GetUserLocalAsync();
    }
}