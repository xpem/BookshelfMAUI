using Bookshelf.Utils;
using CommunityToolkit.Mvvm.Input;
using Services.User;
using System.Windows.Input;

namespace Bookshelf.ViewModels
{
    public partial class SignUpVM(IUserService userBLL) : ViewModelBase
    {

        string name, email;

        public string Name { get => name; set { if (name != value) { SetProperty(ref (name), value); } } }

        public string Email { get => email; set { if (email != value) { SetProperty(ref (email), value); } } }

        string password;

        public string Password { get => password; set { if (password != value) { SetProperty(ref (password), value); } } }

        string confirmPassword;

        public string ConfirmPassword { get => confirmPassword; set { if (confirmPassword != value) { SetProperty(ref (confirmPassword), value); } } }

        bool btnCreateUserIsEnabled = true;

        public bool BtnCreateUserIsEnabled { get => btnCreateUserIsEnabled; set { if (btnCreateUserIsEnabled != value) { SetProperty(ref (btnCreateUserIsEnabled), value); } } }

        private bool VerifyFileds()
        {
            bool validInformation = true;

            if (string.IsNullOrEmpty(Name))
                validInformation = false;


            if (string.IsNullOrEmpty(Email))
                validInformation = false;
            else if (!Validations.ValidateEmail(Email))
            {
                _ = Application.Current.MainPage.DisplayAlert("Aviso", "Digite um email válido", null, "Ok");
                return false;
            }

            if (string.IsNullOrEmpty(Password)) validInformation = false;
            else if (Password.Length < 4) validInformation = false;

            if (string.IsNullOrEmpty(ConfirmPassword)) validInformation = false;
            else if (!ConfirmPassword.Equals(Password, StringComparison.CurrentCultureIgnoreCase)) validInformation = false;

            if (!validInformation)
                _ = Application.Current.MainPage.DisplayAlert("Aviso", "Preencha os campos e confirme a senha corretamente", null, "Ok");

            return validInformation;
        }

        [RelayCommand]
        private async Task SignUp()
        {
            if (!(Connectivity.NetworkAccess == NetworkAccess.Internet))
            {
                _ = await Application.Current.MainPage.DisplayAlert("Aviso", "Sem conexão com a internet", null, "Ok");
                return;
            }

            if (VerifyFileds())
            {
                BtnCreateUserIsEnabled = false;

                //
                Models.Responses.BLLResponse resp = await userBLL.AddUser(name, email, password);

                if (!resp.Success)
                    await Application.Current.MainPage.DisplayAlert("Erro", "Não foi possível cadastrar o usuário!", null, "Ok");
                else
                {
                    bool res = await Application.Current.MainPage.DisplayAlert("Aviso", "Usuário cadastrado!", null, "Ok");

                    if (!res)
                        await Shell.Current.GoToAsync("..");
                }
            }
        }
    }
}