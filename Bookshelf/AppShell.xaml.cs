using Bookshelf.ViewModels;
using Bookshelf.Views;
using Bookshelf.Views.Book;
using Bookshelf.Views.GoogleSearch;

namespace Bookshelf;

public partial class AppShell : Shell
{
    private readonly AppShellVM AppShellVM;

    public AppShell(AppShellVM appShellVM)
    {
        InitializeComponent();

        BindingContext = AppShellVM = appShellVM;
    }

}
