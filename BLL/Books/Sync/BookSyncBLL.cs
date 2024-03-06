using ApiDAL;
using ApiDAL.Interfaces;
using DbContextDAL;
using Models.Books;
using Models.OperationQueue;
using Models.Responses;
using System.Text.Json;

namespace BLL.Books.Sync
{
    public class BookSyncBLL(IBookApiBLL booksApiBLL, IBookDAL bookDAL, IOperationQueueDAL operationQueueDAL) : IBookSyncBLL
    {
        public async Task<(int added, int updated)> ApiToLocalSync(int uid, DateTime lastUpdate)
        {
            int added = 0, updated = 0;

            //update local database
            Models.Responses.BLLResponse respGetBooksByLastUpdate = await booksApiBLL.GetBooksByLastUpdateAsync(lastUpdate);

            if (respGetBooksByLastUpdate.Success && respGetBooksByLastUpdate.Content is not null)
            {
                List<Book>? BooksByLastUpdate = respGetBooksByLastUpdate.Content as List<Book>;

                if (BooksByLastUpdate is not null)
                {
                    //bookshelfDbContext.ChangeTracker.Clear();

                    foreach (Book apiBook in BooksByLastUpdate)
                    {
                        if (apiBook is null) { throw new ArgumentNullException(nameof(apiBook)); }

                        apiBook.UserId = uid;

                        DateTime? bookLastUpdate;
                        if (apiBook.Id is not null)
                            bookLastUpdate = bookDAL.GetBookUpdatedAtById(apiBook.Id.Value);
                        else
                            throw new ArgumentNullException(nameof(apiBook.Id));

                        if (bookLastUpdate == null && !apiBook.Inactive)
                        {
                            await bookDAL.ExecuteAddBookAsync(apiBook);
                            added++;
                        }
                        else if (apiBook.UpdatedAt > bookLastUpdate)
                        {
                            await bookDAL.ExecuteUpdateBookAsync(apiBook);
                            updated++;
                        }

                        if (lastUpdate < apiBook.UpdatedAt) lastUpdate = apiBook.UpdatedAt;
                    }
                }
            }

            return (added, updated);
        }

        public async Task<(int added, int updated)> LocalToApiSync(int uid, DateTime lastUpdate)
        {
            int added = 0, updated = 0;
            List<ApiOperation> pendingOperations = await operationQueueDAL.GetPendingOperationsByStatusAsync(Models.OperationQueue.OperationStatus.Pending);

            foreach (var pendingOperation in pendingOperations)
            {
                if (pendingOperation.ObjectType == Models.OperationQueue.ObjectType.Book)
                {
                    Book? book = JsonSerializer.Deserialize<Book>(pendingOperation.Content);
                    if (book is null) throw new ArgumentNullException(nameof(book));
                    BLLResponse? bLLResponse = null;

                    switch (pendingOperation.ExecutionType)
                    {
                        case ExecutionType.Insert:

                            bLLResponse = await booksApiBLL.AddBookAsync(book);

                            if (bLLResponse.Success)
                            {
                                book.Id = Convert.ToInt32(bLLResponse.Content);
                                await bookDAL.ExecuteUpdateBookAsync(book);
                                added++;
                            }
                            else throw new Exception($"Não foi possivel sincronizar o livro {pendingOperation.ObjectId}, res: {bLLResponse.Error}");
                            break;
                        case ExecutionType.Update:

                            if (book.Id is null)
                            {
                                Book? insertedBook = await bookDAL.GetBookByLocalIdAsync(book.UserId, book.LocalId);

                                if (insertedBook is not null)
                                    bLLResponse = await booksApiBLL.UpdateBookAsync(insertedBook);
                                else throw new NullReferenceException("Livro inserido não encontrado " + book.LocalId);
                            }
                            else
                                bLLResponse = await booksApiBLL.UpdateBookAsync(book);

                            if (bLLResponse.Success)
                                updated++;
                            else throw new Exception($"Não foi possivel sincronizar o livro {pendingOperation.ObjectId}, res: {bLLResponse.Error}");

                            break;
                    }
                }
                else throw new ArgumentException("Invalid ObjecType, Op Id: " + pendingOperation.Id);

                await operationQueueDAL.UpdateOperationStatusAsync(OperationStatus.Success, pendingOperation.Id);
            }
            return (added, updated);
        }
    }
}
