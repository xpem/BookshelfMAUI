using Bookshelf.Services.Sync;
using Bookshelf.Views;
using Models.DTOs;
using Services;
using Services.Books.Historic.Sync;
using Services.Books.Sync;
using Services.User;

namespace Bookshelf.ViewModels
{
    public partial class FirstSyncProcessVM : ViewModelBase
    {
        private decimal progress;
        private readonly ISyncService SyncServices;

        public decimal Progress { get => progress; set { if (progress != value) { SetProperty(ref (progress), value); } } }

        public IUserService UserBLL { get; }
        public IBookSyncService BooksSyncBLL { get; }
        public IBookHistoricSyncBLL BookHistoricSyncBLL { get; }
        public IBuildDbService BuildDbBLL { get; }

        private readonly AppShellVM AppShellVM;

        public FirstSyncProcessVM(IUserService userBLL, IBookSyncService booksSyncBLL, IBookHistoricSyncBLL bookHistoricSyncBLL, ISyncService syncServices, IBuildDbService buildDbBLL, AppShellVM appShellVM)
        {
            UserBLL = userBLL;
            BooksSyncBLL = booksSyncBLL;
            BookHistoricSyncBLL = bookHistoricSyncBLL;
            this.SyncServices = syncServices;
            BuildDbBLL = buildDbBLL;
            AppShellVM = appShellVM;
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

                        _ = AppShellVM.AtualizaUserShowData();

                        _ = Shell.Current.GoToAsync($"//{nameof(Main)}");

                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }
    }
}
