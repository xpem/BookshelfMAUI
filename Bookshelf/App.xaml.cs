using Bookshelf.Utils.Navigation;
using Bookshelf.Views;
using BookshelfServices.Books.Sync;
using BookshelfServices.User;

namespace Bookshelf;

public partial class App : Application
{
    public App(INavigationServices navigationService, IUserServices userServices, IBooksSyncServices booksSyncServices)
    {

        BookshelfServices.BuildDbServices.BuildSQLiteDb();

        InitializeComponent();

        if (userServices.GetUserLocal() != null)
        {
            Thread thread = new(booksSyncServices.SyncLocalDb) { IsBackground = true };
            thread.Start();

            MainPage = new NavigationPage();
            navigationService.NavigateToPage<Main>();
        }
        else
        {
            MainPage = new NavigationPage();
            navigationService.NavigateToPage<Access>();
        }
    }
}
