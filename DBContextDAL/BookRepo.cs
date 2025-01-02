using Microsoft.EntityFrameworkCore;
using Models.Books;
using Models.DTOs;
using Repositories.Interfaces;

namespace Repositories
{
    public class BookRepo(BookshelfDbContext bookshelfDbContext) : IBookRepo
    {
        readonly int pageSize = 10;

        public async Task<int> UpdateAsync(Book book)
        {
            return await bookshelfDbContext.Book.Where(x => x.UserId == book.UserId && x.LocalId == book.LocalId)
                .ExecuteUpdateAsync(y => y
            .SetProperty(z => z.Isbn, book.Isbn)
            .SetProperty(z => z.Id, book.Id)
            .SetProperty(z => z.LocalTempId, book.LocalTempId)
            .SetProperty(z => z.Title, book.Title)
            .SetProperty(z => z.SubTitle, book.SubTitle)
            .SetProperty(z => z.Authors, book.Authors)
            .SetProperty(z => z.Volume, book.Volume)
            .SetProperty(z => z.Pages, book.Pages)
            .SetProperty(z => z.Year, book.Year)
            .SetProperty(z => z.Status, book.Status)
            .SetProperty(z => z.Genre, book.Genre)
            .SetProperty(z => z.Isbn, book.Isbn)
            .SetProperty(z => z.Cover, book.Cover)
            .SetProperty(z => z.GoogleId, book.GoogleId)
            .SetProperty(z => z.Score, book.Score)
            .SetProperty(z => z.Comment, book.Comment)
            .SetProperty(z => z.CreatedAt, book.CreatedAt)
            .SetProperty(z => z.UpdatedAt, book.UpdatedAt)
            .SetProperty(z => z.Inactive, book.Inactive));
        }

        public async Task<List<TotalBooksGroupedByStatus>> GetTotalBooksGroupedByStatusAsync(int uid)
        {
            try
            {
                var books = await bookshelfDbContext.Book.Where(x => x.UserId == uid && !x.Inactive).ToListAsync();

                return books.GroupBy(x => x.Status).Select(x => new TotalBooksGroupedByStatus { Status = x.Key, Count = x.Count() }).ToList();
            }
            catch (Exception ex) { throw; }
        }

        public async Task<Book?> GetBookByLocalIdAsync(int uid, int localId)
            => await bookshelfDbContext.Book.Where(x => x.UserId == uid && x.LocalId == localId).FirstOrDefaultAsync();

        public async Task<Book?> GetBookByLocalTempIdAsync(int uid, string localTempId)
          => await bookshelfDbContext.Book.Where(x => x.UserId == uid && x.LocalTempId == localTempId).FirstOrDefaultAsync();

        public async Task<Book?> GetByTitleAsync(int uid, string title)
            => await bookshelfDbContext.Book.Where(x => x.UserId == uid && x.Title != null && EF.Functions.Like(x.Title, $"%{title}%")).FirstOrDefaultAsync();

        public async Task<Book?> GetByIdAsync(int id) => await bookshelfDbContext.Book.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();

        //public async Task<List<Book>> GetBookByAfterUpdatedAtAsync(int uid, DateTime lastUpdate)
        //    => await bookshelfDbContext.Book.Where(x => x.UserId == uid && x.UpdatedAt > lastUpdate).ToListAsync();

        public async Task<int> CreateAsyn(Book book)
        {
            bookshelfDbContext.Book.Add(book);

            int resp = await bookshelfDbContext.SaveChangesAsync();

            bookshelfDbContext.ChangeTracker?.Clear();

            return resp;
        }

        public async Task<bool> CheckIfExistsWithSameTitleAsync(int uid, string title, int? localId)
        {
            if (localId.HasValue)
                return await bookshelfDbContext.Book.AnyAsync(x => x.UserId == uid && x.Title != null && EF.Functions.Like(x.Title, $"%{title}%") && x.LocalId != localId);
            else
                return await bookshelfDbContext.Book.AnyAsync(x => x.UserId == uid && x.Title != null && EF.Functions.Like(x.Title, $"%{title}%"));
        }

        public async Task<Book?> GetByTitleOrGoogleIdAsync(int uid, string title, string? googleId)
        {
            return await bookshelfDbContext.Book.Where(x => x.UserId == uid && ((x.Title != null && EF.Functions.Like(x.Title, $"%{title}%"))
            || (x.GoogleId != null && x.GoogleId.Equals(googleId)))).FirstOrDefaultAsync();
        }

        public async Task<List<Book>> GetByStatusAsync(int uid, Status status, int page, string? searchTitle)
        {
            if (string.IsNullOrEmpty(searchTitle))
                return await bookshelfDbContext.Book.Where(x => x.UserId == uid && x.Status == status && x.Inactive == false)
                    .OrderByDescending(x => x.UpdatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            else
                return await bookshelfDbContext.Book.Where(x => x.UserId == uid && x.Status == status && x.Inactive == false && x.Title != null && EF.Functions.Like(x.Title, $"%{searchTitle}%"))
                    .OrderByDescending(x => x.UpdatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<List<Book>> GetAsync(int uid, int page, string? searchTitle)
        {
            if (string.IsNullOrEmpty(searchTitle))
                return await bookshelfDbContext.Book.Where(x => x.UserId == uid && x.Inactive == false).OrderBy(x => x.UpdatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            else
                return await bookshelfDbContext.Book.Where(x => x.UserId == uid && x.Inactive == false && x.Title != null && EF.Functions.Like(x.Title, $"%{searchTitle}%")).OrderBy(x => x.UpdatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        }

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
