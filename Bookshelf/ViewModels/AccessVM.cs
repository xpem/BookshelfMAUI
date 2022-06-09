using Bookshelf.Utils.Navigation;
using Bookshelf.ViewModels.Components;
using Bookshelf.Views;
using BookshelfServices.Books.Sync;
using BookshelfServices.User;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.Connectivity;

namespace Bookshelf.ViewModels
{
    public partial class AccessVM : ViewModelBase
    {

        [ObservableProperty]
        string email;

        [ObservableProperty]
        string password;

        [ObservableProperty]
        string signInText;

        [ObservableProperty]
        bool btnSignEnabled;

        readonly IUserServices userServices;
        readonly IBooksSyncServices booksSyncServices;

        public AccessVM(INavigationServices _navigation, IUserServices _userServices, IBooksSyncServices _booksSyncServices)
        {
            navigation = _navigation;
            userServices = _userServices;
            booksSyncServices = _booksSyncServices;
            SignInText = "Acessar";
        }

        [ICommand]
        async Task SignIn()
        {
            if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password))
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    if (Password.Length > 3)
                    {
                        SignInText = "Acessando...";
                        BtnSignEnabled = false;
                        bool resp = false;

                        resp = await userServices.SignIn(Email, Password);

                        if (resp)
                        {
                            Thread thread = new(booksSyncServices.SyncLocalDb) { IsBackground = true };
                            thread.Start();

                            Application.Current.MainPage = new NavigationPage();
                            _ = (Application.Current.MainPage.Navigation).PushAsync(navigation.ResolvePage<Main>(), true);
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("Aviso", "Email/senha incorretos", null, "Ok");
                        }
                        BtnSignEnabled = true;
                        SignInText = "Acessar";
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Aviso", "Digite sua senha", null, "Ok");
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Aviso", "É necessário ter acesso a internet para efetuar o primeiro acesso.", null, "Ok");
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Aviso", "Insira seu email e senha.", null, "Ok");
            }
        }

        [ICommand]
        async Task CreateUser() => await (Application.Current.MainPage.Navigation).PushAsync(navigation.ResolvePage<CreateUser>(), true);

        [ICommand]
        async Task UpdatePassword() => await (Application.Current.MainPage.Navigation).PushAsync(navigation.ResolvePage<UpdatePassword>(), true);

    }
}
