using Bookshelf.ViewModels;

namespace Bookshelf.Views;

public partial class Login : ContentPage
{
    public Login(LoginVM loginVM)
    {
        InitializeComponent();

        BindingContext = loginVM;
    }
}