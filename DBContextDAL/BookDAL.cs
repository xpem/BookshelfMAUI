using DBContextDAL;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Books;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DbContextDAL
{
    public class BookDAL : IBookDAL
    {
        private readonly BookshelfDbContext bookshelfDbContext;

        public BookDAL(BookshelfDbContext bookshelfDbContext)
        {
            this.bookshelfDbContext = bookshelfDbContext;
        }

        public async Task<int> ExecuteUpdateBookAsync(Book book)
        {
            bookshelfDbContext.ChangeTracker?.Clear();
            bookshelfDbContext.Update(book);
            return await bookshelfDbContext.SaveChangesAsync();
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
            => await bookshelfDbContext.Book.Where(x => x.UserId == uid && x.Title != null && x.Title.ToLower().Contains(title.ToLower())).FirstOrDefaultAsync();

        public async Task<DateTime?> GetBookUpdatedAtByIdAsync(int id)
            => await bookshelfDbContext.Book.Where(x => x.Id.Equals(id)).Select(y => y.UpdatedAt).FirstOrDefaultAsync();

        public List<Book> GetBookByAfterUpdatedAt(int uid, DateTime lastUpdate)
            => bookshelfDbContext.Book.Where(x => x.UserId == uid && x.UpdatedAt > lastUpdate).ToList();

        public async Task<int> ExecuteAddBookAsync(Book book)
        {
            bookshelfDbContext.Add(book);
            return await bookshelfDbContext.SaveChangesAsync();
        }

        public async Task<Book?> GetBookbyTitleOrGoogleIdAsync(int uid, string title, string googleId)
            => await bookshelfDbContext.Book.Where(x => x.UserId == uid && ((x.Title != null &&
                   x.Title.ToLower().Equals(title.ToLower())) || (x.GoogleId != null && x.GoogleId.Equals(googleId)))).FirstOrDefaultAsync();

        public async Task<List<Book>> GetBooksByStatusAsync(int uid, Status status)
                => await bookshelfDbContext.Book.Where(x => x.UserId == uid && x.Status == status && x.Inactive == false).OrderByDescending(x => x.UpdatedAt).ToListAsync();

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
