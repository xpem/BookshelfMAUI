using BLL.Books.Historic.Sync;
using BLL.Books.Sync;
using BLL.User;
using Models;

namespace Bookshelf.Services.Sync
{
    public class SyncServices(IUserService userBLL, IBookSyncService booksSyncBLL, IBookHistoricSyncBLL bookHistoricSyncBLL) : ISyncServices
    {
        public static SyncStatus Synchronizing { get; set; }

        public Timer Timer { get; set; }

        //40 secs
        readonly int Interval = 30000;

        public bool ThreadIsRunning { get; set; } = false;

        public void StartThread()
        {
            if (!ThreadIsRunning)
            {
                Synchronizing = SyncStatus.Sleeping;

                Thread thread = new(SetTimer) { IsBackground = true };
                thread.Start();
            }
        }

        private void SetTimer()
        {
            if (!ThreadIsRunning)
            {
                ThreadIsRunning = true;
                SyncLocalDb(null);

                Timer = new Timer(SyncLocalDb, null, Interval, Timeout.Infinite);
            }
        }

        public async void SyncLocalDb(object state) => await ExecSyncAsync();

        public async Task ExecSyncAsync()
        {
            try
            {
                Models.User user = userBLL.GetUserLocal().Result;

                if (user != null && Synchronizing != SyncStatus.Processing)
                {
                    Synchronizing = SyncStatus.Processing;

                    if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                    {
                        await booksSyncBLL.LocalToApiSync(user.Id, user.LastUpdate);

                        await booksSyncBLL.ApiToLocalSync(user.Id, user.LastUpdate);

                        await bookHistoricSyncBLL.ApiToLocalSync(user.Id, user.LastUpdate);

                        userBLL.UpdateLocalUserLastUpdate(user.Id);
                    }

                    Synchronizing = SyncStatus.Sleeping;
                }
            }
            catch (HttpRequestException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("No connection could be made because the target machine actively refused it."))
                    Synchronizing = SyncStatus.ServerOff;
                else throw ex;
            }
            catch (UnauthorizedAccessException ex)
            {
                Synchronizing = SyncStatus.Unauthorized;
            }
            catch
            {
                throw;
            }
            finally
            {
                Timer?.Change(Interval, Timeout.Infinite);

                if (Synchronizing != SyncStatus.Unauthorized)
                    Synchronizing = SyncStatus.Sleeping;
            }
        }
    }
}
