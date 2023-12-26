using DBContextDAL;
using Microsoft.EntityFrameworkCore;
using Models;

namespace BLL
{
    public class BuildDbBLL(BookshelfDbContext bookshelfDBContext) : IBuildDbBLL
    {
        public async Task Init()
        {
            await bookshelfDBContext.Database.EnsureCreatedAsync();

            VersionDbTables? actualVesionDbTables = await bookshelfDBContext.VersionDbTables.FirstOrDefaultAsync();

            VersionDbTables newVersionDbTables = new() { Id = 0, VERSION = 8 };

            if (actualVesionDbTables != null)
            {
                if (actualVesionDbTables.VERSION != newVersionDbTables.VERSION)
                {
                    await bookshelfDBContext.Database.EnsureDeletedAsync();

                    await bookshelfDBContext.Database.EnsureCreatedAsync();
                }
            }
            else
            {
                bookshelfDBContext.VersionDbTables.Add(newVersionDbTables);
                await bookshelfDBContext.SaveChangesAsync();
            }
        }

        public async Task CleanLocalDatabase()
        {
            bookshelfDBContext.Book.RemoveRange(bookshelfDBContext.Book);
            bookshelfDBContext.User.RemoveRange(bookshelfDBContext.User);
            bookshelfDBContext.BookHistoric.RemoveRange(bookshelfDBContext.BookHistoric);
            bookshelfDBContext.BookHistoricItem.RemoveRange(bookshelfDBContext.BookHistoricItem);

            await bookshelfDBContext.SaveChangesAsync();
        }
    }
}
