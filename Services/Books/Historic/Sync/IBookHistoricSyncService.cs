namespace Services.Books.Historic.Sync
{
    public interface IBookHistoricSyncService
    {
        Task ApiToLocalSync(int uid, DateTime lastUpdate);
    }
}