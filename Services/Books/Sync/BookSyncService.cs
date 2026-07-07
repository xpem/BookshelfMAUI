using Models.DTOs;
using Models.Exceptions;
using Models.Responses;
using Repos;
using Repos.Interfaces;
using Services.Books.Interfaces;

namespace Services.Books.Sync
{
    public class BookSyncService(IBookApiService booksApiBLL, IBookRepo bookDAL, ISyncCursorRepo syncCursorRepo) : IBookSyncService
    {
        private const int PAGEMAX = 50;

        public async Task<DateTime> GetLastUpdatedAtAsync()
        {
            return await syncCursorRepo.GetAsync(SyncCursorKeys.Book);
        }

        public async Task ApiToLocalSync(int uid, DateTime lastUpdate)
        {
            // Use server-anchored cursor instead of device-clock based lastUpdate
            DateTime cursor = await syncCursorRepo.GetAsync(SyncCursorKeys.Book);

            // If cursor has a value, use it; otherwise fall back to the legacy lastUpdate
            DateTime effectiveLastUpdate = cursor > DateTime.MinValue ? cursor : lastUpdate;

            int page = 1;
            DateTime maxServerTs = effectiveLastUpdate;

            while (true)
            {
                ServiceResponse respGetBooksByLastUpdate = await booksApiBLL.GetByLastUpdateAsync(effectiveLastUpdate, page);

                if (respGetBooksByLastUpdate != null && respGetBooksByLastUpdate.Success && respGetBooksByLastUpdate.Content is not null)
                {
                    List<Book>? booksByLastUpdate = respGetBooksByLastUpdate.Content as List<Book>;

                    if (booksByLastUpdate is not null)
                    {
                        foreach (Book apiBook in booksByLastUpdate)
                        {
                            if (apiBook is null) throw new ArgumentNullException(nameof(apiBook));

                            apiBook.UserId = uid;

                            await ApplyFromApiAsync(apiBook, uid);

                            // Track highest server timestamp
                            if (apiBook.UpdatedAt > maxServerTs)
                                maxServerTs = apiBook.UpdatedAt;
                        }

                        if (booksByLastUpdate.Count < PAGEMAX)
                            break;
                    }
                    else break;
                }
                else throw new BookshelfAPIException("Erro ao tentar utilizar a api do UniqueServer/bookshelf/books: " + respGetBooksByLastUpdate?.ErrorMessage);

                page++;
            }

            // Advance cursor to highest server-side UpdatedAt
            if (maxServerTs > effectiveLastUpdate)
                await syncCursorRepo.SaveAsync(SyncCursorKeys.Book, maxServerTs);
        }

        /// <summary>
        /// Applies a book received from the API to the local database using GUID-based matching.
        /// </summary>
        private async Task ApplyFromApiAsync(Book apiBook, int uid)
        {
            Book? localBook = null;

            // 1. Try to match by BookId (GUID) first
            if (apiBook.BookId is not null && apiBook.BookId != Guid.Empty)
            {
                localBook = await bookDAL.GetByBookIdAsync(apiBook.BookId.Value, uid);
            }

            // 2. Fall back to ExternalId (int Id) lookup
            if (localBook is null && apiBook.Id is not null)
            {
                localBook = await bookDAL.GetByIdAsync(apiBook.Id.Value);
            }

            if (localBook is not null)
            {
                // Skip if currently being pushed (prevent race condition)
                if (localBook.SyncStatus == BookSyncStatus.Pushing)
                {
                    System.Diagnostics.Debug.WriteLine($"[BookSyncService] Skipping pull of book {localBook.LocalId}: currently being pushed.");
                    return;
                }

                // Last-writer-wins: update only if server UpdatedAt > local UpdatedAt
                if (apiBook.UpdatedAt > localBook.UpdatedAt)
                {
                    apiBook.LocalId = localBook.LocalId;
                    apiBook.SyncStatus = BookSyncStatus.Synced;

                    // Preserve BookId from server if local is empty
                    if ((!localBook.BookId.HasValue || localBook.BookId == Guid.Empty) && apiBook.BookId.HasValue && apiBook.BookId != Guid.Empty)
                        apiBook.BookId = apiBook.BookId;
                    else if (!apiBook.BookId.HasValue || apiBook.BookId == Guid.Empty)
                        apiBook.BookId = localBook.BookId;

                    await bookDAL.UpdateAsync(apiBook);
                }
                else
                {
                    // Even if not updating, persist BookId from server if local is missing
                    if ((!localBook.BookId.HasValue || localBook.BookId == Guid.Empty) && apiBook.BookId.HasValue && apiBook.BookId != Guid.Empty)
                    {
                        localBook.BookId = apiBook.BookId;
                        localBook.Id = apiBook.Id;
                        await bookDAL.UpdateAsync(localBook);
                    }
                }
            }
            else
            {
                // No match — insert new record
                if (!apiBook.Inactive)
                {
                    apiBook.SyncStatus = BookSyncStatus.Synced;
                    if (!apiBook.BookId.HasValue || apiBook.BookId == Guid.Empty)
                        apiBook.BookId = Guid.NewGuid();

                    await bookDAL.CreateAsync(apiBook);
                }
            }
        }

        public async Task LocalToApiSync()
        {
            // Legacy: now handled by PushPendingAsync(uid) called from SyncService.
            // Kept for interface backward compatibility.
            // No-op: the SyncService now calls PushPendingAsync(uid) directly.
        }

        /// <summary>
        /// Pushes all pending books for a specific user.
        /// </summary>
        public async Task PushPendingAsync(int uid)
        {
            var pending = await bookDAL.GetPendingPushAsync(uid);

            foreach (var book in pending)
            {
                try
                {
                    await PushAsync(book);
                }
                catch
                {
                    // Individual push failed — keep as Pending, retry next cycle
                    continue;
                }
            }
        }

        private async Task PushAsync(Book book)
        {
            // Skip if already has ExternalId and is not really pending
            // (edge case protection)
            if (book.SyncStatus != BookSyncStatus.Pending)
                return;

            // Mark as Pushing — pull must not overwrite during this window
            await bookDAL.SetSyncStatusAsync(book.LocalId, BookSyncStatus.Pushing);

            try
            {
                ServiceResponse response;

                if (book.Id is not null && book.Id > 0)
                {
                    // Already has server ID — PUT to update
                    response = await booksApiBLL.UpdateAsync(book);

                    if (response.Success)
                    {
                        await bookDAL.SetSyncStatusAsync(book.LocalId, BookSyncStatus.Synced);
                    }
                    else
                    {
                        await bookDAL.SetSyncStatusAsync(book.LocalId, BookSyncStatus.Pending);
                        throw new Exception($"Failed to update book {book.LocalId}: {response.Content}");
                    }
                }
                else
                {
                    // No server ID — POST (server does GUID-based upsert)
                    response = await booksApiBLL.CreateAsync(book);

                    if (response.Success && response.Content is not null)
                    {
                        int serverId = Convert.ToInt32(response.Content);
                        await bookDAL.SetExternalIdAndSyncedAsync(book.LocalId, serverId);
                    }
                    else
                    {
                        await bookDAL.SetSyncStatusAsync(book.LocalId, BookSyncStatus.Pending);
                        throw new Exception($"Failed to push book {book.LocalId}: {response.Content}");
                    }
                }
            }
            catch
            {
                // Ensure we revert to Pending on any failure
                await bookDAL.SetSyncStatusAsync(book.LocalId, BookSyncStatus.Pending);
                throw;
            }
        }
    }
}
