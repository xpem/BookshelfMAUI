namespace BLL.Books.Sync
{
    public interface IBookSyncService
    {
        Task LocalToApiSync(int uid, DateTime lastUpdate);

        Task ApiToLocalSync(int uid, DateTime lastUpdate);
    }
}
