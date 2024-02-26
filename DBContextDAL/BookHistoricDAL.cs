using DBContextDAL;
using Microsoft.EntityFrameworkCore;
using Models.Books.Historic;

namespace DbContextDAL
{
    public class BookHistoricDAL : IBookHistoricDAL
    {
        private readonly BookshelfDbContext bookshelfDbContext;
        readonly int pageSize = 10;

        public BookHistoricDAL(BookshelfDbContext bookshelfDbContext)
        {
            this.bookshelfDbContext = bookshelfDbContext;
        }

        public async Task<List<BookHistoric>> GetBookHistoricByBookIdAsync(int uid, int bookId, int page) => 
            await bookshelfDbContext.BookHistoric.Where(x => x.Uid == uid && x.BookId == bookId)
            .Include(x => x.BookHistoricItems).OrderByDescending(x => x.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        public async Task<int> ExecuteAddBookHistoricAsync(BookHistoric bookHistoric, int uid)
        {
            if (bookshelfDbContext.BookHistoric.Where(x => x.Id == bookHistoric.Id).ToList().Count == 0)
            {
                bookHistoric.Uid = uid;

                await bookshelfDbContext.BookHistoric.AddAsync(bookHistoric);

                if (bookHistoric.BookHistoricItems is not null)
                    foreach (BookHistoricItem _bookHistoricItem in bookHistoric.BookHistoricItems)
                    {
                        if (((await bookshelfDbContext.BookHistoricItem.Where(x => x.Id == _bookHistoricItem.Id).ToListAsync()).Count) == 0)
                        {
                            _bookHistoricItem.Uid = uid;
                            await bookshelfDbContext.BookHistoricItem.AddAsync(_bookHistoricItem);
                        }
                    }

                int resp = await bookshelfDbContext.SaveChangesAsync();

                bookshelfDbContext.ChangeTracker?.Clear();

                return resp;
            }

            return 0;
        }
    }
}
