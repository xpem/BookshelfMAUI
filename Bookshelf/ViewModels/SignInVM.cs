using Bookshelf.ViewModels.Components;
using Bookshelf.Views;
using BLL.Books.Sync;
using BLL.User;
using Plugin.Connectivity;
using System.Windows.Input;

namespace Bookshelf.ViewModels
{
    public class SignInVM : ViewModelBase
    {

        string email, password, signInText;

        public string Email { get => email; set { if (email != value) { email = value; OnPropertyChanged(); } } }

        public string Password { get => password; set { if (password != value) { password = value; OnPropertyChanged(); } } }

        public string SignInText { get => signInText; set { if (signInText != value) { signInText = value; OnPropertyChanged(); } } }

        bool btnSignEnabled = true;

        public bool BtnSignEnabled { get => btnSignEnabled; set { if (btnSignEnabled != value) { btnSignEnabled = value; OnPropertyChanged(); } } }

        readonly IBooksSyncBLL booksSyncServices;

        public SignInVM(IBooksSyncBLL _booksSyncServices)
        {
            booksSyncServices = _booksSyncServices;
            SignInText = "Acessar";
        }

        public ICommand SignInCommand => new Command(async () =>
         {
             IsBusy = true;
             try
             {
                 if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password))
                 {
                     if (CrossConnectivity.Current.IsConnected)
                     {
                         if (Password.Length > 3)
                         {
                             SignInText = "Acessando...";
                             BtnSignEnabled = false;
                            var resp = await UserBLL.GetUser(Email, Password);

                             if (resp.Success)
                             {
                                 booksSyncServices.StartThread();

                                 await Shell.Current.GoToAsync($"//{nameof(Main)}");

                                 //Application.Current.MainPage = new NavigationPage();
                                 //_ = (Application.Current.MainPage.Navigation).PushAsync(navigation.ResolvePage<Main>(), true);
                             }
                             else
                                 await Application.Current.MainPage.DisplayAlert("Aviso", "Email/senha incorretos", null, "Ok");

                             BtnSignEnabled = true;
                             SignInText = "Acessar";
                             IsBusy = false;
                         }
                         else
                             await Application.Current.MainPage.DisplayAlert("Aviso", "Digite sua senha", null, "Ok");

                     }
                     else
                         await Application.Current.MainPage.DisplayAlert("Aviso", "É necessário ter acesso a internet para efetuar o primeiro acesso.", null, "Ok");

                 }
                 else
                     await Application.Current.MainPage.DisplayAlert("Aviso", "Insira seu email e senha.", null, "Ok");
             }
             catch { throw; }

         });

        public ICommand CreateUserCommand => new Command(async () => await Shell.Current.GoToAsync($"{nameof(SignUp)}"));

        public ICommand UpdatePasswordCommand => new Command(async () => await Shell.Current.GoToAsync($"{nameof(UpdatePassword)}"));

    }
}
