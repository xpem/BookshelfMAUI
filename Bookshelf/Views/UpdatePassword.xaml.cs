using Bookshelf.ViewModels;

namespace Bookshelf.Views;

public partial class UpdatePassword : ContentPage
{
	public UpdatePassword(UpdatePasswordVM updatePasswordVM)
	{
		InitializeComponent();
        BindingContext = updatePasswordVM;
    }
}