using Models;

namespace Bookshelf.Services.Sync
{
    public interface ISyncServices
    {
        SyncStatus Synchronizing { get; set; }

        bool ThreadIsRunning { get; set; }

        Timer Timer { get; set; }

        void StartThread();
    }
}