using Microsoft.EntityFrameworkCore;
using Models.DTOs.OperationQueue;
using Repos.Interfaces;

namespace Repos
{
    public class OperationQueueRepo(IDbContextFactory<BookshelfDbContext> bookshelfDbContext) : IOperationQueueRepo
    {
        public async Task UpdateOperationStatusAsync(ApiOperationStatus operationStatus, int operationId)
        {
            using var context = bookshelfDbContext.CreateDbContext();
            await context.ApiOperationQueue.Where(x => x.Id == operationId)
            .ExecuteUpdateAsync(y => y
            .SetProperty(z => z.Status, operationStatus)
            .SetProperty(z => z.UpdatedAt, DateTime.Now));
        }

        public async Task<List<ApiOperation>> GetPendingOperationsByStatusAsync(ApiOperationStatus operationStatus)
        {
            using var context = bookshelfDbContext.CreateDbContext();
            return await context.ApiOperationQueue.Where(x => x.Status == operationStatus).OrderBy(x => x.CreatedAt).ToListAsync();
        }

        public async Task InsertOperationInQueueAsync(ApiOperation apiOperation)
        {
            using var context = bookshelfDbContext.CreateDbContext();
            context.ApiOperationQueue.Add(apiOperation);

            await context.SaveChangesAsync();
        }

        public async Task<bool> CheckIfHasPendingOperationWithObjectId(string objectId)
        {
            using var context = bookshelfDbContext.CreateDbContext();
            return await context.ApiOperationQueue.AnyAsync(x => x.ObjectId == objectId && x.Status == ApiOperationStatus.Pending);
        }

        public async Task<bool> CheckIfHasPendingOperation()
        {
            using var context = bookshelfDbContext.CreateDbContext();
            return await context.ApiOperationQueue.AnyAsync(x => x.Status == ApiOperationStatus.Pending);
        }
    }
}
