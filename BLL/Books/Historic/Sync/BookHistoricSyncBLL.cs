using BLL.Books.Historic.Interfaces;
using DBContextDAL;
using Models.Books.Historic;

namespace BLL.Books.Historic.Sync
{
    public class BookHistoricSyncBLL : IBookHistoricSyncBLL
    {

        readonly IBookHistoricApiBLL BookHistoricApiBLL;
        private readonly BookshelfDbContext bookshelfDbContext;

        public BookHistoricSyncBLL(IBookHistoricApiBLL bookHistoricApiBLL, BookshelfDbContext bookshelfDbContext)
        {
            BookHistoricApiBLL = bookHistoricApiBLL;
            this.bookshelfDbContext = bookshelfDbContext;
        }

        public async Task ApiToLocalSync(int uid, DateTime lastUpdate)
        {
            Models.Responses.BLLResponse respGetBookHistoricListByCreatedAt = await BookHistoricApiBLL.GetBookHistoricByLastCreatedAt(lastUpdate);

            if (respGetBookHistoricListByCreatedAt.Success && respGetBookHistoricListByCreatedAt.Content is not null)
            {
                List<BookHistoric>? bookHistoricsList = respGetBookHistoricListByCreatedAt.Content as List<BookHistoric>;

                //bookshelfDbContext.ChangeTracker.Clear();

                if (bookHistoricsList is not null)
                    foreach (BookHistoric bookHistoric in bookHistoricsList)
                    {
                        if (bookshelfDbContext.BookHistoric.Where(x => x.Id == bookHistoric.Id).ToList().Count == 0)
                        {
                            bookHistoric.Uid = uid;

                            bookshelfDbContext.BookHistoric.Add(bookHistoric);

                            if (bookHistoric.BookHistoricItems is not null)
                                foreach (BookHistoricItem _bookHistoricItem in bookHistoric.BookHistoricItems)
                                {
                                    if ((bookshelfDbContext.BookHistoricItem.Where(x => x.Id == _bookHistoricItem.Id).ToList().Count) == 0)
                                    {
                                        _bookHistoricItem.Uid = uid;
                                        bookshelfDbContext.BookHistoricItem.Add(_bookHistoricItem);
                                    }
                                }

                            await bookshelfDbContext.SaveChangesAsync();
                        }
                    }
            }
        }
    }
}
