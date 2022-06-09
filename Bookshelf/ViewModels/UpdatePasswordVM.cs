using Bookshelf.Utils;
using Bookshelf.Utils.Navigation;
using Bookshelf.ViewModels.Components;
using BookshelfServices.User.AuthServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bookshelf.ViewModels
{
    public partial class UpdatePasswordVM : ViewModelBase
    {
        [ObservableProperty]
        string email;

        readonly IUserAuthServices userAuthServices;

        public UpdatePasswordVM(INavigationServices _navigation, IUserAuthServices _userAuthServices)
        {
            navigation = _navigation;
            userAuthServices = _userAuthServices;
        }

        [ICommand]
        async Task UpdatePassword()
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
        }
    }
}
