using BLL.Books.Historic.Sync;
using BLL.Books.Sync;
using BLL.User;
using Bookshelf.Services.Sync;
using Bookshelf.ViewModels.Components;
using Bookshelf.Views;

namespace Bookshelf.ViewModels
{
    public class FirstSyncProcessVM : ViewModelBase
    {
        private decimal progress;
        private readonly ISyncServices syncServices;

        public decimal Progress { get => progress; set { if (progress != value) { progress = value; OnPropertyChanged(nameof(Progress)); } } }

        public IUserService UserBLL { get; }
        public IBookSyncService BooksSyncBLL { get; }
        public IBookHistoricSyncBLL BookHistoricSyncBLL { get; }

        public FirstSyncProcessVM(IUserService userBLL, IBookSyncService booksSyncBLL, IBookHistoricSyncBLL bookHistoricSyncBLL, ISyncServices syncServices)
        {
            UserBLL = userBLL;
            BooksSyncBLL = booksSyncBLL;
            BookHistoricSyncBLL = bookHistoricSyncBLL;
            this.syncServices = syncServices;
            _ = SynchronizingProcess();
        }


        private async Task SynchronizingProcess()
        {
            try
            {
                Models.User user = await UserBLL.GetUserLocal();

                if (user != null)
                {
                    if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                    {

                        await BooksSyncBLL.LocalToApiSync(user.Id, user.LastUpdate);

                        Progress = 0.25M;

                        await BooksSyncBLL.ApiToLocalSync(user.Id, user.LastUpdate);

                        Progress = 0.5M;

                        await BookHistoricSyncBLL.ApiToLocalSync(user.Id, user.LastUpdate);

                        Progress = 0.75M;

                        UserBLL.UpdateLocalUserLastUpdate(user.Id);

                        Progress = 1;

                        _ = Task.Run(() => { Task.Delay(5000); syncServices.StartThread(); });

                        _ = Shell.Current.GoToAsync($"//{nameof(Main)}");

                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }
    }
}
