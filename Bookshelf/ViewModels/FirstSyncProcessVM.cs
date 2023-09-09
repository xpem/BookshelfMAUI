using BLL.Books.Historic.Sync;
using BLL.Books.Sync;
using BLL.User;
using Bookshelf.ViewModels.Components;
using Bookshelf.Views;

namespace Bookshelf.ViewModels
{
    public class FirstSyncProcessVM : ViewModelBase
    {
        private decimal progress;

        public decimal Progress { get => progress; set { if (progress != value) { progress = value; OnPropertyChanged(nameof(Progress)); } } }

        public IUserBLL UserBLL { get; }
        public IBookSyncBLL BooksSyncBLL { get; }
        public IBookHistoricSyncBLL BookHistoricSyncBLL { get; }

        public FirstSyncProcessVM(IUserBLL userBLL, IBookSyncBLL booksSyncBLL, IBookHistoricSyncBLL bookHistoricSyncBLL)
        {
            UserBLL = userBLL;
            BooksSyncBLL = booksSyncBLL;
            BookHistoricSyncBLL = bookHistoricSyncBLL;

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

                        await UserBLL.UpdateLocalUserLastUpdate(user);

                        Progress = 1;

                        _ = Shell.Current.GoToAsync($"//{nameof(Main)}");
                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }



    }
}
