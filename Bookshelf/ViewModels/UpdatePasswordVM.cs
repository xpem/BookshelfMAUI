using Bookshelf.Utils;
using CommunityToolkit.Mvvm.Input;
using Services.User;
using System.Windows.Input;

namespace Bookshelf.ViewModels
{
    public partial class UpdatePasswordVM(IUserService userBLL) : ViewModelBase
    {

        string email;

        public string Email { get => email; set { if (email != value) { SetProperty(ref (email), value); } } }

        [RelayCommand]
        private async Task UpdatePassword()
        {
            if (!(Connectivity.NetworkAccess == NetworkAccess.Internet))
            {
                await Application.Current.Windows[0].Page.DisplayAlert("Aviso", "Sem conexão com a internet", null, "Ok");
                return;
            }

            if (string.IsNullOrEmpty(Email))
            {
                await Application.Current.Windows[0].Page.DisplayAlert("Aviso", "Digite um email válido", null, "Ok");
                return;
            }
            else if (!Validations.ValidateEmail(Email))
            {
                await Application.Current.Windows[0].Page.DisplayAlert("Aviso", "Digite um email válido", null, "Ok");
                return;
            }
            else
            {
                _ = userBLL.RecoverPassword(Email);

                await Application.Current.Windows[0].Page.DisplayAlert("Aviso", "Email de alteração de senha enviado!", null, "Ok");

                await Shell.Current.GoToAsync("..");
            }
        }
    }
}
