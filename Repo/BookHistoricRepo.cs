using Microsoft.EntityFrameworkCore;
using Models.DTOs;
using Repos.Interfaces;

namespace Repos
{
    public class BookHistoricRepo(IDbContextFactory<BookshelfDbContext> bookshelfDbContext) : IBookHistoricRepo
    {
        readonly int pageSize = 10;

        public async Task<List<BookHistoric>> GetBookHistoricByBookIdAsync(int uid, int bookId, int page)
        {
            using var context = bookshelfDbContext.CreateDbContext();
            return await context.BookHistoric.Where(x => x.Uid == uid && x.BookId == bookId)
              .Include(x => x.BookHistoricItems).OrderByDescending(x => x.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<List<BookHistoric>> Get(int uid, int page)
        {
            using var context = bookshelfDbContext.CreateDbContext();
            return await context.BookHistoric.Where(x => x.Uid == uid)
            .Include(x => x.BookHistoricItems)
            .Select(x => new BookHistoric
            {
                BookId = x.BookId,
                Id = x.BookId,
                BookHistoricItems = x.BookHistoricItems,
                CreatedAt = x.CreatedAt,
                LocalId = x.LocalId,
                TypeId = x.TypeId,
                TypeName = x.TypeName,
                Uid = x.Uid,
                BookTitle = context.Book.Where(y => y.UserId == uid && y.Id == x.BookId).Single().Title
            })
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<int> ExecuteAddBookHistoricAsync(BookHistoric bookHistoric, int uid)
        {
            try
            {
                using var context = bookshelfDbContext.CreateDbContext();
                if ((await context.BookHistoric.Where(x => x.Id == bookHistoric.Id).ToListAsync()).Count == 0)
                {
                    bookHistoric.Uid = uid;

                    await context.BookHistoric.AddAsync(bookHistoric);

                    if (bookHistoric.BookHistoricItems is not null)
                        foreach (BookHistoricItem _bookHistoricItem in bookHistoric.BookHistoricItems)
                        {
                            if ((await context.BookHistoricItem.Where(x => x.Id == _bookHistoricItem.Id).ToListAsync()).Count == 0)
                            {
                                _bookHistoricItem.Uid = uid;
                                await context.BookHistoricItem.AddAsync(_bookHistoricItem);
                            }
                        }

                    return await context.SaveChangesAsync();
                }
            }
            catch (Exception ex) { throw; }

            return 0;
        }
    }
}
