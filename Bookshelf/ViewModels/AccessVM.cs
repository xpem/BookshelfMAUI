using Bookshelf.Utils.Navigation;
using Bookshelf.Views;
using BookshelfServices.Books.Sync;
using BookshelfServices.User;
using Plugin.Connectivity;
using System.Windows.Input;

namespace Bookshelf.ViewModels
{
    public class AccessVM : ViewModelBase
    {

        private string email, password;

        public string Email { get => email; set { email = value; OnPropertyChanged(); } }
        public string Password { get => password; set { password = value; OnPropertyChanged(); } }

        private bool btnRegisterAccessEnabled;

        public bool BtnRegisterAccessEnabled { get => btnRegisterAccessEnabled; set { btnRegisterAccessEnabled = value; OnPropertyChanged(); } }

        readonly IUserServices userServices;
        readonly IBooksSyncServices booksSyncServices;

        public AccessVM(INavigationServices _navigation, IUserServices _userServices, IBooksSyncServices _booksSyncServices)
        {
            navigation = _navigation;
            userServices = _userServices;
            booksSyncServices = _booksSyncServices;
        }

        public ICommand SignInCommand
        {
            get
            {
                return new Command(async (e) =>
                {
                    if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password))
                    {
                        if (CrossConnectivity.Current.IsConnected)
                        {
                            if (Password.Length > 3)
                            {
                                BtnRegisterAccessEnabled = false;
                                bool resp = false;

                                resp = await userServices.SignIn(Email, Password);

                                if (resp)
                                {
                                    Thread thread = new(booksSyncServices.SyncLocalDb) { IsBackground = true };
                                    thread.Start();

                                    Application.Current.MainPage = new NavigationPage();
                                    await navigation.NavigateToPage<Main>();
                                }
                                else
                                {
                                    await Application.Current.MainPage.DisplayAlert("Aviso", "Usuário/senha incorretos", null, "Ok");
                                }
                                BtnRegisterAccessEnabled = true;
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
                        await Application.Current.MainPage.DisplayAlert("Aviso", "Insira seu nome e senha.", null, "Ok");
                    }
                });
            }
        }

        public ICommand CreateUserCommand => new Command((e) => { });//navigation.PushAsync(new InsertUser()); });

        public ICommand UpdatePasswordCommand => new Command((e) => { });//navigation.PushAsync(new UpdatePassword()); });

    }
}
