namespace Services.Books.Sync
{
    public interface IBookSyncService
    {
        Task LocalToApiSync();

        Task ApiToLocalSync(int uid, DateTime lastUpdate);
    }
}
