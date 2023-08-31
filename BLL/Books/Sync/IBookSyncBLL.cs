namespace BLL.Books.Sync
{
    public interface IBookSyncBLL
    {
        Task<(int added, int updated)> LocalToApiSync(int uid, DateTime lastUpdate);

        Task<(int added, int updated)> ApiToLocalSync(int uid, DateTime lastUpdate);
    }
}
