using Bookshelf.Messages;
using Bookshelf.Services.Sync;
using Bookshelf.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Models.DTOs;
using Services;
using Services.User;
using System.Windows.Input;

namespace Bookshelf.ViewModels
{
    public partial class AppShellVM(ISyncService syncService, IBuildDbService BuildDbService, IUserService userService, IUserSessionService userSessionService) : ObservableObject
    {
        string email, name;

        public string Email { get => email; set { if (email != value) { SetProperty(ref (email), value); } } }

        public string Name { get => name; set { if (name != value) { SetProperty(ref (name), value); } } }


        public void Init()
        {
            WeakReferenceMessenger.Default.Register<UserLoggedInMessage>(this, async (r, m) =>
            {
                await UserFlyoutAsync();
            });
        }

        public async Task UserFlyoutAsync()
        {
            try
            {
                UserDTO? user = await userSessionService.GetCurrentUserAsync();

                if (user is not null)
                {
                    Name = user.Name;
                    Email = user.Email;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log the error, show a message to the user, etc.)
                Console.WriteLine($"Error fetching user data: {ex.Message}");
            }
        }

        public async Task AtualizaUserShowData()
        {
            UserDTO user = await userService.GetAsync();

            if (user is not null)
            {
                Name = user.Name;
                Email = user.Email;
            }
        }

        [RelayCommand]
        private async Task SignOut()
        {
            bool resp = await Application.Current.Windows[0].Page.DisplayAlert("Confirmação", "Deseja sair e retornar a tela inicial?", "Sim", "Cancelar");

            if (resp)
            {
                //finalize sync thread process
                syncService.ThreadIsRunning = false;

                syncService.Timer?.Dispose();

                (App.Current as App).Uid = 0;

                await BuildDbService.CleanLocalDatabase();

                _ = Shell.Current.GoToAsync($"//{nameof(SignIn)}");
            }
        }
    }
}
