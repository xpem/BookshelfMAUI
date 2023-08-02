namespace LocalDbDAL.User
{
    public interface IUserLocalDAL
    {
        Task InsertUser(Models.User user);

        Task<Models.User?> GetUser();

        Task UpdateUserLastUpdateLocal(int? id, DateTime lastUpdate);

        Task UpdateToken(int? id, string? token);

        Task<string?> GetUserToken();
    }
}
