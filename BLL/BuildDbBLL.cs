using Microsoft.EntityFrameworkCore;
using Models.DTOs;
using Repos;

namespace Services
{
    public class BuildDbBLL(IDbContextFactory<BookshelfDbContext> DbCtx) : IBuildDbService
    {
        public void Init()
        {
            using var context = DbCtx.CreateDbContext();
            context.Database.EnsureCreated();

            VersionDbTables? actualVesionDbTables = context.VersionDbTables.FirstOrDefault();

            VersionDbTables newVersionDbTables = new() { Id = 0, VERSION = 18 };

            if (actualVesionDbTables != null)
            {
                if (actualVesionDbTables.VERSION != newVersionDbTables.VERSION)
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();

                    actualVesionDbTables.VERSION = newVersionDbTables.VERSION;

                    context.VersionDbTables.Add(actualVesionDbTables);

                    context.SaveChanges();
                }
            }
            else
            {
                context.VersionDbTables.Add(newVersionDbTables);

                context.SaveChanges();
            }


        }

        public async Task CleanLocalDatabase()
        {
            using var context = DbCtx.CreateDbContext();

            context.Book.RemoveRange(context.Book);
            context.User.RemoveRange(context.User);
            context.BookHistoric.RemoveRange(context.BookHistoric);
            context.BookHistoricItem.RemoveRange(context.BookHistoricItem);
            context.ApiOperationQueue.RemoveRange(context.ApiOperationQueue);

            await context.SaveChangesAsync();
        }
    }
}
