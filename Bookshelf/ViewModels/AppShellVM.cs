using Bookshelf.Services.Sync;
using Bookshelf.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models.DTOs;
using Services;
using Services.User;
using System.Windows.Input;

namespace Bookshelf.ViewModels
{
    public partial class AppShellVM(ISyncService syncService, IBuildDbService buildDbBLL, IUserService userService) : ObservableObject
    {
        string email, name;

        public string Email { get => email; set { if (email != value) { SetProperty(ref (email), value); } } }

        public string Name { get => name; set { if (name != value) { SetProperty(ref (name), value); } } }


        public async Task AtualizaUserShowData()
        {
            User user = await userService.GetUserLocal();

            if (user is not null)
            {
                Name = user.Name;
                Email = user.Email;
            }
        }

        [RelayCommand]
        private async Task SignOut()
        {
            bool resp = await Application.Current.MainPage.DisplayAlert("Confirmação", "Deseja sair e retornar a tela inicial?", "Sim", "Cancelar");

            if (resp)
            {
                //finalize sync thread process
                syncService.ThreadIsRunning = false;

                syncService.Timer?.Dispose();

                (App.Current as App).Uid = 0;

                await buildDbBLL.CleanLocalDatabase();

                _ = Shell.Current.GoToAsync($"//{nameof(SignIn)}");
            }
        }
    }
}
