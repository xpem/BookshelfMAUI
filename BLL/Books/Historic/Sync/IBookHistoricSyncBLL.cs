namespace BLL.Books.Historic.Sync
{
    public interface IBookHistoricSyncBLL
    {
        Task ApiToLocalSync(int uid, DateTime lastUpdate);
    }
}