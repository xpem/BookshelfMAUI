using Microsoft.EntityFrameworkCore;
using Models.DTOs;

namespace Repos
{
    public interface ISyncCursorRepo
    {
        /// <summary>
        /// Returns the server timestamp anchor for the given entity, or DateTime.MinValue
        /// when no pull has been completed yet (triggers a full pull).
        /// </summary>
        Task<DateTime> GetAsync(string entityName);

        /// <summary>
        /// Persists the highest server-side timestamp seen in the last pull response.
        /// Creates the row if it does not exist yet.
        /// </summary>
        Task SaveAsync(string entityName, DateTime serverTimestamp);
    }

    public class SyncCursorRepo(IDbContextFactory<BookshelfDbContext> dbCtx) : ISyncCursorRepo
    {
        public async Task<DateTime> GetAsync(string entityName)
        {
            using var db = dbCtx.CreateDbContext();
            var cursor = await db.SyncCursor.FirstOrDefaultAsync(c => c.EntityName == entityName);
            return cursor?.ServerTimestamp ?? DateTime.MinValue;
        }

        public async Task SaveAsync(string entityName, DateTime serverTimestamp)
        {
            using var db = dbCtx.CreateDbContext();
            var cursor = await db.SyncCursor.FirstOrDefaultAsync(c => c.EntityName == entityName);

            if (cursor is null)
            {
                db.SyncCursor.Add(new SyncCursor
                {
                    EntityName = entityName,
                    ServerTimestamp = serverTimestamp,
                });
            }
            else
            {
                cursor.ServerTimestamp = serverTimestamp;
                db.SyncCursor.Update(cursor);
            }

            await db.SaveChangesAsync();
        }
    }
}
