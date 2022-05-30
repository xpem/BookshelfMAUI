using Bookshelf.Utils.Navigation;
using Bookshelf.Views;
using System.Windows.Input;

namespace Bookshelf.ViewModels
{
    public class MainVM : ViewModelBase
    {

        private string illRead, reading, read, interrupted, isSync, isConnected;

        public string IllRead { get => illRead; set { illRead = value; OnPropertyChanged(); } }

        public string Reading { get => reading; set { reading = value; OnPropertyChanged(); } }

        public string Read { get => read; set { read = value; OnPropertyChanged(); } }

        public string Interrupted { get => interrupted; set { interrupted = value; OnPropertyChanged(); } }

        public string IsSync { get => isSync; set { isSync = value; OnPropertyChanged(); } }

        public string IsConnected { get => isConnected; set { isConnected = value; OnPropertyChanged(); } }

        public MainVM(INavigationServices _navigation)
        {
            navigation = _navigation;
        }

        public ICommand LogoutCommand => new Command(async (e) =>
        {
            bool resp = await Application.Current.MainPage.DisplayAlert("Confirmação", "Deseja sair e retornar a tela inicial?", "Sim", "Cancelar");

            if (resp)
            {
                BookshelfServices.User.UserServices.CleanUserDatabase();
                Application.Current.MainPage = new NavigationPage();
                await navigation.NavigateToPage<Access>();
            }
        });

    }
}
