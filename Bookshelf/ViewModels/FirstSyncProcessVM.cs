using Bookshelf.Messages;
using Bookshelf.Services.Sync;
using Bookshelf.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Models.DTOs;
using Services;
using Services.Books.Historic.Sync;
using Services.Books.Sync;
using Services.User;

namespace Bookshelf.ViewModels
{
    public partial class FirstSyncProcessVM(IUserService userService, IBookSyncService booksSyncService, IBookHistoricSyncService bookHistoricSyncService, ISyncService syncService, IBuildDbService buildDbService, AppShellVM appShellVM) : ViewModelBase
    {
        [ObservableProperty] private double progress;

        public async Task SynchronizingProcess()
        {
            try
            {
                UserDTO user = await userService.GetAsync();
                DateTime mindate = DateTime.MinValue;

                if (user != null)
                {
                    if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                    {
                        await booksSyncService.ApiToLocalSync(user.Id, mindate);

                        Progress = 0.25;

                        await booksSyncService.PushPendingAsync(user.Id);

                        Progress = 0.5;

                        await bookHistoricSyncService.ApiToLocalSync(user.Id, mindate);

                        Progress = 0.75;

                        userService.UpdateLocalUserLastUpdate(user.Id);

                        Progress = 1;

                        _ = Task.Run(async () => { await Task.Delay(5000); syncService.StartThread(); });

                        WeakReferenceMessenger.Default.Send(new UserLoggedInMessage());

                        _ = Shell.Current.GoToAsync($"//{nameof(Main)}");

                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }
    }
}
