using Bookshelf.Services.Sync;
using Bookshelf.ViewModels.Components;
using Bookshelf.Views;
using Models.DTOs;
using Services;
using Services.Books.Historic.Sync;
using Services.Books.Sync;
using Services.User;

namespace Bookshelf.ViewModels
{
    public class FirstSyncProcessVM : ViewModelBase
    {
        private decimal progress;
        private readonly ISyncServices SyncServices;

        public decimal Progress { get => progress; set { if (progress != value) { progress = value; OnPropertyChanged(nameof(Progress)); } } }

        public IUserService UserBLL { get; }
        public IBookSyncService BooksSyncBLL { get; }
        public IBookHistoricSyncBLL BookHistoricSyncBLL { get; }
        public IBuildDbBLL BuildDbBLL { get; }

        public FirstSyncProcessVM(IUserService userBLL, IBookSyncService booksSyncBLL, IBookHistoricSyncBLL bookHistoricSyncBLL, ISyncServices syncServices, IBuildDbBLL buildDbBLL)
        {
            UserBLL = userBLL;
            BooksSyncBLL = booksSyncBLL;
            BookHistoricSyncBLL = bookHistoricSyncBLL;
            this.SyncServices = syncServices;
            BuildDbBLL = buildDbBLL;
            _ = SynchronizingProcess();
        }


        private async Task SynchronizingProcess()
        {
            try
            {
                User user = await UserBLL.GetUserLocal();

                if (user != null)
                {
                    if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                    {
                        await BooksSyncBLL.ApiToLocalSync(user.Id, user.LastUpdate);                       

                        Progress = 0.25M;

                        await BooksSyncBLL.LocalToApiSync(user.Id, user.LastUpdate);

                        Progress = 0.5M;

                        await BookHistoricSyncBLL.ApiToLocalSync(user.Id, user.LastUpdate);

                        Progress = 0.75M;

                        UserBLL.UpdateLocalUserLastUpdate(user.Id);

                        Progress = 1;

                        _ = Task.Run(() => { Task.Delay(5000); SyncServices.StartThread(); });

                        _ = Shell.Current.GoToAsync($"//{nameof(Main)}");

                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }
    }
}
