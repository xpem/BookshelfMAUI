using Bookshelf.Views;

namespace Bookshelf;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(AddBook), typeof(AddBook));

        Routing.RegisterRoute(nameof(BookDetail), typeof(BookDetail));

        Routing.RegisterRoute(nameof(AddUser), typeof(AddUser));

        Routing.RegisterRoute(nameof(UpdatePassword), typeof(UpdatePassword));

        Routing.RegisterRoute(nameof(BookList), typeof(BookList));
        //Routing.RegisterRoute(nameof(Main), typeof(Main));
    }
}
