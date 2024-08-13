using Bookshelf.ViewModels;
using Bookshelf.Views;
using Bookshelf.Views.Book;
using Bookshelf.Views.GoogleSearch;

namespace Bookshelf;

public partial class AppShell : Shell
{
    public AppShell(AppShellVM appShellVM)
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(AddBook), typeof(AddBook));

        Routing.RegisterRoute(nameof(BookDetail), typeof(BookDetail));

        Routing.RegisterRoute(nameof(SignUp), typeof(SignUp));

        Routing.RegisterRoute(nameof(SignIn), typeof(SignIn));

        Routing.RegisterRoute(nameof(UpdatePassword), typeof(UpdatePassword));

        Routing.RegisterRoute(nameof(BookList), typeof(BookList));

        Routing.RegisterRoute(nameof(GoogleBooksResults), typeof(GoogleBooksResults));

        Routing.RegisterRoute(nameof(Historic), typeof(Historic));

        Routing.RegisterRoute(nameof(BookHistoric), typeof(BookHistoric));

        Routing.RegisterRoute(nameof(FirstSyncProcess), typeof(FirstSyncProcess));

        Routing.RegisterRoute(nameof(Main), typeof(Main));

        BindingContext = appShellVM;
    }
}
