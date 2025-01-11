using Bookshelf.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Services.User;
using System.Windows.Input;

namespace Bookshelf.ViewModels
{
    public partial class SignInVM(IUserService userBLL) : ViewModelBase
    {
        private string email, password, signInText = "Acessar";

        bool btnSignEnabled = true;

        private string version = ((App)Application.Current).Version;

        public string Version { get => version; set { if (version != value) { SetProperty(ref (version), value); } } }

        public string Email
        {
            get => email;
            set
            {
                if (email != value)
                {
                    SetProperty(ref (email), value);
                }
            }
        }

        public string Password { get => password; set { if (password != value) { SetProperty(ref (password), value); } } }

        public string SignInText { get => signInText; set { if (signInText != value) { SetProperty(ref (signInText), value); } } }

        public bool BtnSignEnabled { get => btnSignEnabled; set { if (btnSignEnabled != value) { SetProperty(ref (btnSignEnabled), value); } } }

        [RelayCommand]
        private async Task SignIn()
        {
            IsBusy = true;
            try
            {
                if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password))
                {
                    if ((Connectivity.NetworkAccess == NetworkAccess.Internet))
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

                                await Application.Current.Windows[0].Page.DisplayAlert("Aviso", errorMessage, null, "Ok");
                            }
                            BtnSignEnabled = true;
                            SignInText = "Acessar";
                            IsBusy = false;
                        }
                        else
                            await Application.Current.Windows[0].Page.DisplayAlert("Aviso", "Digite sua senha", null, "Ok");

                    }
                    else
                        await Application.Current.Windows[0].Page.DisplayAlert("Aviso", "É necessário ter acesso a internet para efetuar o primeiro acesso.", null, "Ok");

                }
                else
                    await Application.Current.Windows[0].Page.DisplayAlert("Aviso", "Insira seu email e senha.", null, "Ok");
            }
            catch { throw; }

        }

        [RelayCommand]
        private async Task CreateUser() => await Shell.Current.GoToAsync($"{nameof(SignUp)}");

        [RelayCommand]
        private async Task UpdatePassword() => await Shell.Current.GoToAsync($"{nameof(UpdatePassword)}");

    }
}
