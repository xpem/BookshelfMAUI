using Bookshelf.Utils;
using Bookshelf.Utils.Navigation;
using Bookshelf.ViewModels.Components;
using BookshelfServices.User.AuthServices;
using System.Windows.Input;

namespace Bookshelf.ViewModels
{
    public class UpdatePasswordVM : ViewModelBase
    {

        string email;

        public string Email { get => email; set { if (email != value) { email = value; OnPropertyChanged(); } } }

        readonly IUserAuthServices userAuthServices;

        public UpdatePasswordVM(INavigationServices _navigation, IUserAuthServices _userAuthServices)
        {
            navigation = _navigation;
            userAuthServices = _userAuthServices;
        }

        public ICommand UpdatePasswordCommand => new Command(async () =>
        {
            if (!(Connectivity.NetworkAccess == NetworkAccess.Internet))
            {
                await Application.Current.MainPage.DisplayAlert("Aviso", "Sem conexão com a internet", null, "Ok");
                return;
            }

            if (string.IsNullOrEmpty(Email))
            {
                await Application.Current.MainPage.DisplayAlert("Aviso", "Digite um email válido", null, "Ok");
                return;
            }
            else if (!Validations.ValidateEmail(Email))
            {
                await Application.Current.MainPage.DisplayAlert("Aviso", "Digite um email válido", null, "Ok");
                return;
            }
            else
            {
                _ = userAuthServices.SendPasswordResetEmail(Email);

                await Application.Current.MainPage.DisplayAlert("Aviso", "Email de alteração de senha enviado!", null, "Ok");

                _ = navigation.NavigateBack();
            }
        });
    }
}
