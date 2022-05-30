using Bookshelf.Utils;
using Bookshelf.Utils.Navigation;
using BookshelfServices.User.AuthServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Bookshelf.ViewModels
{
    public class UpdatePasswordVM : ViewModelBase
    {
        private string email;

        public string Email { get => email; set { email = value; OnPropertyChanged(); } }

        readonly IUserAuthServices userAuthServices;

        public UpdatePasswordVM(INavigationServices _navigation, IUserAuthServices _userAuthServices)
        {
            navigation = _navigation;
            userAuthServices = _userAuthServices;
        }

        public ICommand UpdatePasswordCommand
        {
            get
            {
                return new Command(async (e) =>
                {
                    if (!CheckInternet())
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
    }
}
