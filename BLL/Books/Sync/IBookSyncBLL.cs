namespace BLL.Books.Sync
{
    public interface IBookSyncBLL
    {
        Task LocalToApiSync(int uid, DateTime lastUpdate);

        Task ApiToLocalSync(int uid, DateTime lastUpdate);
    }
}
