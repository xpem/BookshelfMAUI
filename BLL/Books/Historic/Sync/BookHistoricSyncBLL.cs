using BLL.Books.Historic.Interfaces;
using LocalDbDAL.Books.BookHistoric;
using Models.Books.Historic;

namespace BLL.Books.Historic.Sync
{
    public class BookHistoricSyncBLL : IBookHistoricSyncBLL
    {

        readonly IBookHistoricApiBLL BookHistoricApiBLL;
        readonly IBookHistoricLocalDAL BookHistoricLocalDAL;

        public BookHistoricSyncBLL(IBookHistoricApiBLL bookHistoricApiBLL, IBookHistoricLocalDAL bookHistoricLocalDAL)
        {
            BookHistoricApiBLL = bookHistoricApiBLL;
            BookHistoricLocalDAL = bookHistoricLocalDAL;
        }

        public async Task ApiToLocalSync(int uid, DateTime lastUpdate)
        {
            Models.Responses.BLLResponse respGetBookHistoricListByCreatedAt = await BookHistoricApiBLL.GetBookHistoricByLastCreatedAt(lastUpdate);

            if (respGetBookHistoricListByCreatedAt.Success && respGetBookHistoricListByCreatedAt.Content is not null)
            {
                List<BookHistoric>? bookHistoricsList = respGetBookHistoricListByCreatedAt.Content as List<BookHistoric>;

                if (bookHistoricsList is not null)
                    foreach (BookHistoric bookHistoric in bookHistoricsList)
                    {
                        if (!await BookHistoricLocalDAL.CheckBookHistoricById(bookHistoric.Id))
                            await BookHistoricLocalDAL.AddBookHistoric(bookHistoric, uid);
                    }
            }
        }
    }
}
