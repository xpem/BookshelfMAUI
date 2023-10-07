using DBContextDAL;
using Microsoft.EntityFrameworkCore;
using Models.Books.Historic;

namespace DbContextDAL
{
    public class BookHistoricDAL : IBookHistoricDAL
    {
        private readonly BookshelfDbContext bookshelfDbContext;

        public BookHistoricDAL(BookshelfDbContext bookshelfDbContext)
        {
            this.bookshelfDbContext = bookshelfDbContext;
        }

        public List<BookHistoric> GetBookHistoricByBookId(int uid, int bookId)
            => bookshelfDbContext.BookHistoric.Where(x => x.Uid == uid && x.BookId == bookId)
            .Include(x => x.BookHistoricItems).OrderByDescending(x => x.CreatedAt).ToList();

        public async Task<int> ExecuteAddBookHistoricAsync(BookHistoric bookHistoric, int uid)
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

                return await bookshelfDbContext.SaveChangesAsync();
            }

            return 0;
        }
    }
}
