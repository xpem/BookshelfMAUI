﻿using DBContextDAL;
using Microsoft.EntityFrameworkCore;
using Models.OperationQueue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbContextDAL
{
    public interface IOperationQueueDAL
    {
        Task<List<ApiOperation>> GetPendingOperationsByStatusAsync(OperationStatus operationStatus);

        Task InsertOperationInQueueAsync(ApiOperation apiOperation);

        Task UpdateOperationStatusAsync(OperationStatus operationStatus, int operationId);

        Task<bool> CheckIfHasPendingOperationWithObjectId(string objectId);
    }

    public class OperationQueueDAL(BookshelfDbContext bookshelfDbContext) : IOperationQueueDAL
    {
        public async Task UpdateOperationStatusAsync(OperationStatus operationStatus, int operationId) =>
            await bookshelfDbContext.ApiOperationQueue.Where(x => x.Id == operationId)
            .ExecuteUpdateAsync(y => y
            .SetProperty(z => z.Status, operationStatus)
            .SetProperty(z => z.UpdatedAt, DateTime.Now));

        public async Task<List<ApiOperation>> GetPendingOperationsByStatusAsync(OperationStatus operationStatus) =>
            await bookshelfDbContext.ApiOperationQueue.Where(x => x.Status == operationStatus).OrderBy(x => x.CreatedAt).ToListAsync();

        public async Task InsertOperationInQueueAsync(Models.OperationQueue.ApiOperation apiOperation)
        {
            bookshelfDbContext.ApiOperationQueue.Add(apiOperation);

            await bookshelfDbContext.SaveChangesAsync();

            bookshelfDbContext.ChangeTracker?.Clear();
        }

        public async Task<bool> CheckIfHasPendingOperationWithObjectId(string objectId) => await bookshelfDbContext.ApiOperationQueue.AnyAsync(x => x.ObjectId == objectId && x.Status == OperationStatus.Pending);
    }


}
