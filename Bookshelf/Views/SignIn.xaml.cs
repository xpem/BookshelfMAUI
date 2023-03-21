using Bookshelf.ViewModels;

namespace Bookshelf.Views;

public partial class SignIn : ContentPage
{
    public SignIn(SignInVM loginVM)
    {
        InitializeComponent();

        BindingContext = loginVM;
    }
}