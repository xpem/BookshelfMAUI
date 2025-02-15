using Bookshelf.ViewModels;

namespace Bookshelf;

public partial class AppShell : Shell
{

    public AppShell(AppShellVM appShellVM)
    {
        InitializeComponent();

        BindingContext = appShellVM;
    }

}
