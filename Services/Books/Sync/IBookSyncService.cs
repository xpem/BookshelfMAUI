namespace Services.Books.Sync
{
    public interface IBookSyncService
    {
        Task LocalToApiSync();

        Task ApiToLocalSync(int uid, DateTime lastUpdate);

        /// <summary>
        /// Pushes all pending books for a specific user to the server.
        /// </summary>
        Task PushPendingAsync(int uid);

        /// <summary>
        /// Returns the last server-anchored timestamp used for delta-pull.
        /// </summary>
        Task<DateTime> GetLastUpdatedAtAsync();
    }
}
