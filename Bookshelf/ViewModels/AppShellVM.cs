using BLL;
using Bookshelf.Services.Sync;
using Bookshelf.Views;
using Models;
using System.Windows.Input;

namespace Bookshelf.ViewModels
{
    public class AppShellVM : BindableObject
    {
        string email, name;

        public string Email { get => email; set { if (email != value) { email = value; OnPropertyChanged(nameof(Email)); } } }

        public string Name { get => name; set { if (name != value) { name = value; OnPropertyChanged(nameof(Name)); } } }

        public ISyncServices SyncService { get; set; }

        public IBuildDbBLL BuildDbBLL { get; set; }

        public AppShellVM(User user, ISyncServices syncService, IBuildDbBLL buildDbBLL)
        {
            SyncService = syncService;
            BuildDbBLL = buildDbBLL;

            if (user is not null)
            {
                Name = user.Name;
                Email = user.Email;
            }
        }

        public ICommand SignOutCommand => new Command(async (e) =>
        {
            bool resp = await Application.Current.MainPage.DisplayAlert("Confirmação", "Deseja sair e retornar a tela inicial?", "Sim", "Cancelar");

            if (resp)
            {
                //finalize sync thread process
                SyncService.ThreadIsRunning = false;

                SyncService.Timer?.Dispose();

                ((App)App.Current).Uid = 0;

                await BuildDbBLL.CleanLocalDatabase();

                _ = Shell.Current.GoToAsync($"//{nameof(SignIn)}");
            }
        });
    }
}
