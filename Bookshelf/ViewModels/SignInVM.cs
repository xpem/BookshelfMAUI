using BLL.User;
using Bookshelf.ViewModels.Components;
using Bookshelf.Views;
using Plugin.Connectivity;
using System.Windows.Input;

namespace Bookshelf.ViewModels
{
    public class SignInVM(IUserBLL userBLL) : ViewModelBase
    {
        string email, password, signInText = "Acessar";

        bool btnSignEnabled = true;

        public string Email { get => email; set { if (email != value) { email = value; OnPropertyChanged(); } } }

        public string Password { get => password; set { if (password != value) { password = value; OnPropertyChanged(); } } }

        public string SignInText { get => signInText; set { if (signInText != value) { signInText = value; OnPropertyChanged(); } } }

        public bool BtnSignEnabled { get => btnSignEnabled; set { if (btnSignEnabled != value) { btnSignEnabled = value; OnPropertyChanged(); } } }

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

                             //Task.Run(BuildDbBLL.Init).Wait();

                             Models.Responses.BLLResponse resp = await userBLL.SignIn(Email, Password);


                             if (resp.Success)
                             {
                                 if (resp.Content is not null and int)
                                     ((App)App.Current).Uid = (int)resp.Content;

                                 await Shell.Current.GoToAsync($"{nameof(FirstSyncProcess)}", false);

                                 //Application.Current.MainPage = new NavigationPage();
                                 //_ = (Application.Current.MainPage.Navigation).PushAsync(navigation.ResolvePage<Main>(), true);
                             }
                             else
                             {
                                 string errorMessage = "";

                                 if (resp.Error == Models.Responses.ErrorTypes.WrongEmailOrPassword)
                                     errorMessage = "Email/senha incorretos";
                                 else if (resp.Error == Models.Responses.ErrorTypes.ServerUnavaliable)
                                     errorMessage = "Servidor indisponível, favor entrar em contato com o desenvolvedor.";
                                 else errorMessage = "Erro não mapeado, favor entrar em contato com o desenvolvedor.";

                                 await Application.Current.MainPage.DisplayAlert("Aviso", errorMessage, null, "Ok");
                             }
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
