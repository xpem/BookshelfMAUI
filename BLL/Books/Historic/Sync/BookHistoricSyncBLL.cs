using BLL.Books.Historic.Interfaces;
using DbContextDAL;
using Models.Books.Historic;

namespace BLL.Books.Historic.Sync
{
    public class BookHistoricSyncBLL(IBookHistoricApiBLL bookHistoricApiBLL, IBookHistoricDAL bookHistoricDAL) : IBookHistoricSyncBLL
    {
        public async Task ApiToLocalSync(int uid, DateTime lastUpdate)
        {
            Models.Responses.BLLResponse respGetBookHistoricListByCreatedAt = await bookHistoricApiBLL.GetBookHistoricByLastCreatedAt(lastUpdate);

            if (respGetBookHistoricListByCreatedAt.Success && respGetBookHistoricListByCreatedAt.Content is not null)
            {
                List<BookHistoric>? bookHistoricsList = respGetBookHistoricListByCreatedAt.Content as List<BookHistoric>;

                //bookshelfDbContext.ChangeTracker.Clear();

                if (bookHistoricsList is not null)
                    foreach (BookHistoric bookHistoric in bookHistoricsList)
                    {
                        await bookHistoricDAL.ExecuteAddBookHistoricAsync(bookHistoric, uid);
                    }
            }
        }
    }
}
