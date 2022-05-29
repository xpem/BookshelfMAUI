using Bookshelf.Utils.Navigation;
using Bookshelf.Views;
using System.Windows.Input;

namespace Bookshelf.ViewModels
{
    public class AccessVM : ViewModelBase
    {
        public AccessVM(INavigationServices _navigation)
        {
            navigation = _navigation;
        }

        public ICommand SignInCommand
        {
            get
            {
                return new Command(async (e) =>
                {
                    Application.Current.MainPage = new NavigationPage();
                    await navigation.NavigateToPage<Main>();
                });
            }
        }

        public ICommand InsertUserCommand => new Command((e) =>
        {
            //navigation.NavigateToPage<InsertUser>();
        });

        //public ICommand UpdatePasswordCommand => new Command((e) => { navigation.PushAsync(new UpdatePassword()); });

    }
}
