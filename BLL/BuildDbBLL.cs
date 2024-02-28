using DBContextDAL;
using Microsoft.EntityFrameworkCore;
using Models;

namespace BLL
{
    public class BuildDbBLL(BookshelfDbContext bookshelfDBContext) : IBuildDbBLL
    {
        public void Init()
        {
            bookshelfDBContext.Database.EnsureCreated();

            VersionDbTables? actualVesionDbTables = bookshelfDBContext.VersionDbTables.FirstOrDefault();

            VersionDbTables newVersionDbTables = new() { Id = 0, VERSION = 9 };

            if (actualVesionDbTables != null)
            {
                if (actualVesionDbTables.VERSION != newVersionDbTables.VERSION)
                {
                    bookshelfDBContext.Database.EnsureDeleted();
                    bookshelfDBContext.Database.EnsureCreated();

                    bookshelfDBContext.VersionDbTables.Update(newVersionDbTables);
                }
            }
            else
            {
                bookshelfDBContext.VersionDbTables.Add(newVersionDbTables);
            }

            bookshelfDBContext.SaveChanges();
        }

        public async Task CleanLocalDatabase()
        {
            bookshelfDBContext.Book.RemoveRange(bookshelfDBContext.Book);
            bookshelfDBContext.User.RemoveRange(bookshelfDBContext.User);
            bookshelfDBContext.BookHistoric.RemoveRange(bookshelfDBContext.BookHistoric);
            bookshelfDBContext.BookHistoricItem.RemoveRange(bookshelfDBContext.BookHistoricItem);
            bookshelfDBContext.ApiOperationQueue.RemoveRange(bookshelfDBContext.ApiOperationQueue);

            await bookshelfDBContext.SaveChangesAsync();
        }
    }
}
