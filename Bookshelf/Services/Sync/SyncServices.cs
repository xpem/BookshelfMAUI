using BLL.Books.Historic.Sync;
using BLL.Books.Sync;
using BLL.User;
using Models;

namespace Bookshelf.Services.Sync
{
    public class SyncServices : ISyncServices
    {
        public SyncStatus Synchronizing { get; set; }

        public Timer Timer { get; set; }

        //40 secs
        readonly int Interval = 40000;

        public bool ThreadIsRunning { get; set; } = false;

        readonly IUserBLL UserBLL;
        readonly IBookSyncBLL BookSyncBLL;
        readonly IBookHistoricSyncBLL BookHistoricSyncBLL;

        public SyncServices(IUserBLL userBLL, IBookSyncBLL booksSyncBLL, IBookHistoricSyncBLL bookHistoricSyncBLL)
        {
            UserBLL = userBLL;
            BookSyncBLL = booksSyncBLL;
            BookHistoricSyncBLL = bookHistoricSyncBLL;
        }

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

        private async void SyncLocalDb(object state)
        {
            try
            {
                Models.User user = await UserBLL.GetUserLocal();

                if (user != null && Synchronizing != SyncStatus.Processing)
                {
                    Synchronizing = SyncStatus.Processing;

                    if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                    {
                        await BookSyncBLL.LocalToApiSync(user.Id, user.LastUpdate);

                        await BookSyncBLL.ApiToLocalSync(user.Id, user.LastUpdate);

                        await BookHistoricSyncBLL.ApiToLocalSync(user.Id, user.LastUpdate);

                        await UserBLL.UpdateLocalUserLastUpdate(user.Id, user.LastUpdate);
                    }

                    Synchronizing = SyncStatus.Sleeping;
                }
            }
            catch (HttpRequestException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("No connection could be made because the target machine actively refused it."))
                { Synchronizing = SyncStatus.ServerOff; }
                else throw ex;
            }
            catch { throw; }
            finally
            {
                if (Timer != null)
                    Timer.Change(Interval, Timeout.Infinite);

                Synchronizing = SyncStatus.Sleeping;
            }
        }

    }

}
