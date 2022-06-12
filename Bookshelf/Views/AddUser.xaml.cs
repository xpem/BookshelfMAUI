using Bookshelf.ViewModels;

namespace Bookshelf.Views;

public partial class AddUser : ContentPage
{
    public AddUser(AddUserVM addUserVM)
    {
        InitializeComponent();

        BindingContext = addUserVM;

    }
}