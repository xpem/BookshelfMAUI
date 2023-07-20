using Bookshelf.Views;
using Bookshelf.Views.GoogleSearch;

namespace Bookshelf;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(AddBook), typeof(AddBook));

        Routing.RegisterRoute(nameof(BookDetail), typeof(BookDetail));

        Routing.RegisterRoute(nameof(SignUp), typeof(SignUp));

        Routing.RegisterRoute(nameof(UpdatePassword), typeof(UpdatePassword));

        Routing.RegisterRoute(nameof(BookList), typeof(BookList));

        Routing.RegisterRoute(nameof(GoogleBooksResults), typeof(GoogleBooksResults));

        Routing.RegisterRoute(nameof(Timeline), typeof(Timeline));

        //Routing.RegisterRoute(nameof(Main), typeof(Main));
    }
}
