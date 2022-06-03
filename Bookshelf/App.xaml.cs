using Bookshelf.Utils.Navigation;
using Bookshelf.Views;
using BookshelfServices.Books.Sync;
using BookshelfServices.User;

namespace Bookshelf;

public partial class App : Application
{

    public App(INavigationServices navigationServices,IUserServices userServices, IBooksSyncServices booksSyncServices)
    {

        BookshelfServices.BuildDbServices.BuildSQLiteDb();

        InitializeComponent();

        if (userServices.GetUserLocal() != null)
        {
            Thread thread = new(booksSyncServices.SyncLocalDb) { IsBackground = true };
            thread.Start();

            MainPage = new NavigationPage();    
            _ = (Current?.MainPage?.Navigation).PushAsync(navigationServices.ResolvePage<Main>(), true);
        }
        else
        {
            MainPage = new NavigationPage();
            _ = (Current?.MainPage?.Navigation).PushAsync(navigationServices.ResolvePage<Access>(), true);
        }
    }
}
