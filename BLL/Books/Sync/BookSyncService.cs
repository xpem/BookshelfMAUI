﻿using Models.DTOs;
using Models.DTOs.OperationQueue;
using Models.Exceptions;
using Models.Responses;
using Repos.Interfaces;
using Services.Books.Interfaces;
using System.Text.Json;

namespace Services.Books.Sync
{
    public class BookSyncService(IBookApiService booksApiBLL, IBookRepo bookDAL, IOperationQueueRepo operationQueueDAL) : IBookSyncService
    {
        private const int PAGEMAX = 50;

        public async Task ApiToLocalSync(int uid, DateTime lastUpdate)
        {
            int page = 1;

            while (true)
            {
                //update local database
                BLLResponse respGetBooksByLastUpdate = await booksApiBLL.GetByLastUpdateAsync(lastUpdate, page);

                if (respGetBooksByLastUpdate != null && respGetBooksByLastUpdate.Success && respGetBooksByLastUpdate.Content is not null)
                {
                    List<Book>? BooksByLastUpdate = respGetBooksByLastUpdate.Content as List<Book>;

                    if (BooksByLastUpdate is not null)
                    {
                        //bookshelfDbContext.ChangeTracker.Clear();

                        foreach (Book apiBook in BooksByLastUpdate)
                        {
                            if (apiBook is null) throw new ArgumentNullException(nameof(apiBook));

                            apiBook.UserId = uid;
                            Book? localBook = null;

                            if (apiBook.Id is not null)
                            {
                                localBook = await bookDAL.GetByIdAsync(apiBook.Id.Value);
                                apiBook.LocalId = localBook?.LocalId ?? 0;
                            }
                            else throw new ArgumentNullException(nameof(apiBook.Id));

                            if (localBook is null && !apiBook.Inactive)
                                await bookDAL.CreateAsync(apiBook);
                            else if (apiBook.UpdatedAt > localBook?.UpdatedAt)
                                await bookDAL.UpdateAsync(apiBook);
                        }

                        if (BooksByLastUpdate.Count < PAGEMAX)
                            break;
                    }
                    else break;
                }
                else throw new BookshelfAPIException("Erro ao tentar utilizar a api do UniqueServer/bookshelf/books: " + respGetBooksByLastUpdate?.ErrorMessage);

                page++;
            }
        }

        public async Task LocalToApiSync()
        {
            List<ApiOperation> pendingOperations = await operationQueueDAL.GetPendingOperationsByStatusAsync(ApiOperationStatus.Pending);

            foreach (var pendingOperation in pendingOperations)
            {
                await operationQueueDAL.UpdateOperationStatusAsync(ApiOperationStatus.Processing, pendingOperation.Id);

                if (pendingOperation.ObjectType == ObjectType.Book)
                {
                    Book? book = JsonSerializer.Deserialize<Book>(pendingOperation.Content);

                    if (book is null) throw new ArgumentNullException(nameof(book));

                    BLLResponse? bLLResponse = null;

                    switch (pendingOperation.ExecutionType)
                    {
                        case ExecutionType.Insert:

                            bLLResponse = await booksApiBLL.CreateAsync(book);

                            if (bLLResponse.Success)
                            {
                                book.Id = Convert.ToInt32(bLLResponse.Content);
                                await bookDAL.UpdateAsync(book);
                            }
                            else throw new Exception($"Não foi possivel sincronizar o livro {pendingOperation.ObjectId}, res: {bLLResponse.Error}");
                            break;
                        case ExecutionType.Update:

                            if (book.Id is null)
                            {
                                Book? insertedBook = await bookDAL.GetBookByLocalIdAsync(book.UserId, book.LocalId);

                                if (insertedBook is not null)
                                    bLLResponse = await booksApiBLL.UpdateAsync(insertedBook);
                                else throw new NullReferenceException("Livro inserido não encontrado " + book.LocalId);
                            }
                            else
                                bLLResponse = await booksApiBLL.UpdateAsync(book);

                            if (!bLLResponse.Success) throw new Exception($"Não foi possivel sincronizar o livro {pendingOperation.ObjectId}, res: {bLLResponse.Error}");

                            break;
                    }
                }
                else throw new ArgumentException("Invalid ObjecType, Op Id: " + pendingOperation.Id);

                await operationQueueDAL.UpdateOperationStatusAsync(ApiOperationStatus.Success, pendingOperation.Id);
            }
        }
    }
}
