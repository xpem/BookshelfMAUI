using Bookshelf.Utils;
using Bookshelf.ViewModels.Components;
using BLL.User;
using System.Windows.Input;

namespace Bookshelf.ViewModels
{
    public class UpdatePasswordVM : ViewModelBase
    {

        string email;

        public string Email { get => email; set { if (email != value) { email = value; OnPropertyChanged(); } } }

        IUserBLL UserBLL;

        public UpdatePasswordVM(IUserBLL userBLL)
        {
            UserBLL = userBLL;
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
                await UserBLL.RecoverPassword(Email);

                await Application.Current.MainPage.DisplayAlert("Aviso", "Email de alteração de senha enviado!", null, "Ok");

                await Shell.Current.GoToAsync("..");
            }
        });
    }
}
