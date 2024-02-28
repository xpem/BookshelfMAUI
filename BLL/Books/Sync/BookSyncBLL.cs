using ApiDAL;
using ApiDAL.Interfaces;
using DbContextDAL;
using Models.Books;
using Models.OperationQueue;
using Models.Responses;

namespace BLL.Books.Sync
{
    public class BookSyncBLL(IBookApiBLL booksApiBLL, IBookDAL bookDAL, IOperationBaseBLL operationBaseBLL, IHttpClientFunctions httpClientFunctions) : IBookSyncBLL
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
            List<ApiOperation> pendingOperations = await operationBaseBLL.GetPendingOperationsByStatusAsync(Models.OperationQueue.OperationStatus.Pending);

            foreach (var pendingOperation in pendingOperations)
            {
                var response = await httpClientFunctions.AuthRequest(pendingOperation.RequestType, ApiKeys.ApiAddress + pendingOperation.Url, pendingOperation.Content);

                if (response.Success && response.Content is not null)
                {
                    if (pendingOperation.ObjectType == Models.OperationQueue.ObjectType.Book)
                    {
                        if (response.Success && response.Content is not null)
                        {
                            if (pendingOperation.ExecutionType == Models.OperationQueue.ExecutionType.Insert)
                            {
                                var bookLocal = await bookDAL.GetBookByLocalTempIdAsync(uid, pendingOperation.ObjectId.ToString())
                                    ?? throw new ArgumentNullException("The local book could not be retrieved.");

                                bookLocal.LocalTempId = null;
                                bookLocal.Id = Convert.ToInt32(response.Content);

                                await bookDAL.ExecuteUpdateBookAsync(bookLocal);
                                added++;
                            }
                            else if (pendingOperation.ExecutionType == Models.OperationQueue.ExecutionType.Update)
                                updated++;

                            await operationBaseBLL.UpdateOperationBookStatusAsync(OperationStatus.Success, pendingOperation.Id);
                        }
                        else throw new Exception($"Não foi possivel sincronizar o livro {pendingOperation.ObjectId}, res: {response.Error}");
                    }
                    else throw new ArgumentException("Invalid ObjecType, Op Id: " + pendingOperation.Id);
                }
            }

            return (added, updated);
        }

        /// <summary>
        /// to do - implementar sincronização in>out por fila de requisições a serem executadas.
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="lastUpdate"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        //public async Task<(int added, int updated)> LocalToApiSync(int uid, DateTime lastUpdate)
        //{
        //    int added = 0, updated = 0;

        //    List<Book> booksList = bookDAL.GetBookByAfterUpdatedAt(uid, lastUpdate);

        //    //update api database
        //    foreach (Book book in booksList)
        //    {
        //        if (book.LocalTempId != null)
        //        {
        //            Models.Responses.BLLResponse addBookResp = await booksApiBLL.AddBookAsync(book);

        //            if (addBookResp.Success && addBookResp.Content is not null)
        //            {
        //                book.LocalTempId = null;
        //                book.Id = Convert.ToInt32(addBookResp.Content);
        //                await bookDAL.ExecuteUpdateBookAsync(book);
        //                added++;
        //            }
        //            else throw new Exception($"Não foi possivel sincronizar o livro {book.Id}, res: {addBookResp.Error}");
        //        }
        //        else
        //        {
        //            Models.Responses.BLLResponse response = await booksApiBLL.UpdateBookAsync(book);
        //            if (response.Success)
        //                updated++;
        //        }
        //    }

        //    return (added, updated);
        //}
    }
}
