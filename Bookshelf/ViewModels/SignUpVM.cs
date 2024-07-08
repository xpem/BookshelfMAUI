using BLL.User;
using Bookshelf.Utils;
using Bookshelf.ViewModels.Components;
using System.Windows.Input;

namespace Bookshelf.ViewModels
{
    public partial class SignUpVM(IUserBLL userBLL) : ViewModelBase
    {

        string name, email;

        public string Name { get => name; set { if (name != value) { name = value; OnPropertyChanged(); } } }

        public string Email { get => email; set { if (email != value) { email = value; OnPropertyChanged(); } } }

        string password;

        public string Password { get => password; set { if (password != value) { password = value; OnPropertyChanged(); } } }

        string confirmPassword;

        public string ConfirmPassword { get => confirmPassword; set { if (confirmPassword != value) { confirmPassword = value; OnPropertyChanged(); } } }

        bool btnCreateUserIsEnabled = true;

        public bool BtnCreateUserIsEnabled { get => btnCreateUserIsEnabled; set { if (btnCreateUserIsEnabled != value) { btnCreateUserIsEnabled = value; OnPropertyChanged(); } } }

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

        public ICommand SignUpCommand => new Command(async () =>
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
        });
    }
}
