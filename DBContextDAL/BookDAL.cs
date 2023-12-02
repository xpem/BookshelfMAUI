using DBContextDAL;
using Microsoft.EntityFrameworkCore;
using Models.Books;

namespace DbContextDAL
{
    public class BookDAL(BookshelfDbContext bookshelfDbContext) : IBookDAL
    {
        public int ExecuteUpdateBook(Book book)
        {
            bookshelfDbContext.ChangeTracker?.Clear();
            bookshelfDbContext.Update(book);
            return bookshelfDbContext.SaveChanges();
        }

        public Totals GetTotalBooksGroupedByStatus(int uid)
        {
            Totals totals = new();

            var list = bookshelfDbContext.Book.Where(x => x.UserId == uid && x.Inactive == false).GroupBy(x => x.Status).Select(x => new { status = x.Key, count = x.Count() }).ToList();

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

        public int ExecuteAddBook(Book book)
        {
            bookshelfDbContext.ChangeTracker?.Clear();
            bookshelfDbContext.Add(book);         
            return bookshelfDbContext.SaveChanges();
        }

        public Book? GetBookByTitleOrGoogleId(int uid, string title, string googleId)
            => bookshelfDbContext.Book.Where(x => x.UserId == uid && ((x.Title != null && EF.Functions.Like(x.Title, $"%{title}%")) || (x.GoogleId != null && x.GoogleId.Equals(googleId)))).FirstOrDefault();
        // x.Title.ToLower().Equals(title.ToLower())
        public List<Book> GetBooksByStatus(int uid, Status status)
        => bookshelfDbContext.Book.Where(x => x.UserId == uid && x.Status == status && x.Inactive == false).OrderByDescending(x => x.UpdatedAt).ToList();

        public async Task<List<Book>> GetBooks(int uid)
                => await bookshelfDbContext.Book.Where(x => x.UserId == uid && x.Inactive == false).OrderBy(x => x.UpdatedAt).ToListAsync();

        public async Task<int> ExecuteInactivateBookAsync(int localId, int userId)
        {
            bookshelfDbContext.ChangeTracker?.Clear();

            bookshelfDbContext.Book.Where(x => x.UserId == userId && x.LocalId == localId).ExecuteUpdate(y => y
            .SetProperty(z => z.Inactive, true)
            .SetProperty(z => z.UpdatedAt, DateTime.Now));

            return await bookshelfDbContext.SaveChangesAsync();
        }

        public async Task<int> ExecuteUpdateBookStatusAsync(int localId, Status status, int score, string comment, int uid)
        {
            bookshelfDbContext.ChangeTracker?.Clear();

            bookshelfDbContext.Book.Where(x => x.UserId == uid && x.LocalId == localId).ExecuteUpdate(y => y
              .SetProperty(z => z.Status, status)
              .SetProperty(z => z.Score, score)
              .SetProperty(z => z.Comment, comment)
              .SetProperty(z => z.UpdatedAt, DateTime.Now));

            return await bookshelfDbContext.SaveChangesAsync();
        }
    }
}
