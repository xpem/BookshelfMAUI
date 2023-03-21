using Bookshelf.ViewModels;

namespace Bookshelf.Views;

public partial class SignUp : ContentPage
{
    public SignUp(SignUpVM signUpVM)
    {
        InitializeComponent();

        BindingContext = signUpVM;

    }
}