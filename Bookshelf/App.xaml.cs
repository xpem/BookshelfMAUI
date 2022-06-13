using Bookshelf.Views;
using BookshelfServices.Books.Sync;
using BookshelfServices.User;

namespace Bookshelf;

public partial class App : Application
{

    public App(IUserServices userServices, IBooksSyncServices booksSyncServices)
    {

        BookshelfServices.BuildDbServices.BuildSQLiteDb();

        InitializeComponent();

        if (userServices.GetUserLocal() != null)
        {
            booksSyncServices.StartThread();

            MainPage = new AppShell();
            Shell.Current.GoToAsync($"//{nameof(Main)}");
        }
        else
        {
            MainPage = new AppShell();
        }
    }
}
