using DBContextDAL;
using Microsoft.EntityFrameworkCore;
using Models.Books;

namespace DbContextDAL
{
    public class BookDAL(BookshelfDbContext bookshelfDbContext) : IBookDAL
    {
        readonly int pageSize = 10;

        public async Task<int> ExecuteUpdateBookAsync(Book book)
        {
            return await bookshelfDbContext.Book.Where(x => x.UserId == book.UserId && x.LocalId == book.LocalId).ExecuteUpdateAsync(y => y
            .SetProperty(z => z.Isbn, book.Isbn)
            .SetProperty(z => z.LocalTempId, book.LocalTempId)
            .SetProperty(z => z.Title, book.Title).SetProperty(z => z.SubTitle, book.SubTitle)
            .SetProperty(z => z.Authors, book.Authors).SetProperty(z => z.Volume, book.Volume)
            .SetProperty(z => z.Pages, book.Pages).SetProperty(z => z.Year, book.Year).SetProperty(z => z.Status, book.Status)
            .SetProperty(z => z.Genre, book.Genre).SetProperty(z => z.Isbn, book.Isbn).SetProperty(z => z.Cover, book.Cover).SetProperty(z => z.GoogleId, book.GoogleId)
            .SetProperty(z => z.Score, book.Score).SetProperty(z => z.Comment, book.Comment)
            .SetProperty(z => z.CreatedAt, book.CreatedAt).SetProperty(z => z.UpdatedAt, book.UpdatedAt).SetProperty(z => z.Inactive, book.Inactive));
        }

        public async Task<Totals> GetTotalBooksGroupedByStatusAsync(int uid)
        {
            Totals totals = new();
            var list = await bookshelfDbContext.Book.Where(x => x.UserId == uid && x.Inactive == false).GroupBy(x => x.Status).Select(x => new { status = x.Key, count = x.Count() }).ToListAsync();

            if (list is not null && list.Count > 0)
            {
                var illRead = list.FirstOrDefault(x => x.status == Status.IllRead);
                var reading = list.FirstOrDefault(x => x.status == Status.Reading);
                var read = list.FirstOrDefault(x => x.status == Status.Read);
                var interrupted = list.FirstOrDefault(x => x.status == Status.Interrupted);

                totals.IllRead = illRead is not null ? illRead.count : 0;
                totals.Reading = reading is not null ? reading.count : 0;
                totals.Read = read is not null ? read.count : 0;
                totals.Interrupted = interrupted is not null ? interrupted.count : 0;
            }
            else
            {
                totals.IllRead = totals.Reading = totals.Read = totals.Interrupted = 0;
            }

            return totals;
        }

        public async Task<Book?> GetBookByLocalIdAsync(int uid, int localId)
            => await bookshelfDbContext.Book.Where(x => x.UserId == uid && x.LocalId == localId).FirstOrDefaultAsync();

        public async Task<Book?> GetBookByTitleAsync(int uid, string title)
            => await bookshelfDbContext.Book.Where(x => x.UserId == uid && x.Title != null && EF.Functions.Like(x.Title, $"%{title}%")).FirstOrDefaultAsync();

        public DateTime? GetBookUpdatedAtById(int id) => bookshelfDbContext.Book.Where(x => x.Id.Equals(id)).FirstOrDefault()?.UpdatedAt;

        public List<Book> GetBookByAfterUpdatedAt(int uid, DateTime lastUpdate)
            => bookshelfDbContext.Book.Where(x => x.UserId == uid && x.UpdatedAt > lastUpdate).ToList();

        public async Task<int> ExecuteAddBookAsync(Book book)
        {
            bookshelfDbContext.Book.Add(book);
            int resp = await bookshelfDbContext.SaveChangesAsync();

            bookshelfDbContext.ChangeTracker?.Clear();
            return resp;
        }

        public Book? GetBookByTitleOrGoogleId(int uid, string title, string googleId)
            => bookshelfDbContext.Book.Where(x => x.UserId == uid && ((x.Title != null && EF.Functions.Like(x.Title, $"%{title}%")) || (x.GoogleId != null && x.GoogleId.Equals(googleId)))).FirstOrDefault();
        // x.Title.ToLower().Equals(title.ToLower())

        public Task< List<Book>> GetBooksByStatusAsync(int uid, Status status, int page)
        => bookshelfDbContext.Book.Where(x => x.UserId == uid && x.Status == status && x.Inactive == false)
            .OrderByDescending(x => x.UpdatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        public async Task<List<Book>> GetBooksAsync(int uid,int page)
                => await bookshelfDbContext.Book.Where(x => x.UserId == uid && x.Inactive == false).OrderBy(x => x.UpdatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        public async Task<int> ExecuteInactivateBookAsync(int localId, int userId)
        {
            return await bookshelfDbContext.Book.Where(x => x.UserId == userId && x.LocalId == localId).ExecuteUpdateAsync(y => y
              .SetProperty(z => z.Inactive, true)
              .SetProperty(z => z.UpdatedAt, DateTime.Now));
        }

        public async Task<int> ExecuteUpdateBookStatusAsync(int localId, Status status, int score, string comment, int uid)
        {
            return await bookshelfDbContext.Book.Where(x => x.UserId == uid && x.LocalId == localId).ExecuteUpdateAsync(y => y
                .SetProperty(z => z.Status, status)
                .SetProperty(z => z.Score, score)
                .SetProperty(z => z.Comment, comment)
                .SetProperty(z => z.UpdatedAt, DateTime.Now));
        }
    }
}
