namespace Services.Books.Historic.Sync
{
    public interface IBookHistoricSyncBLL
    {
        Task ApiToLocalSync(int uid, DateTime lastUpdate);
    }
}