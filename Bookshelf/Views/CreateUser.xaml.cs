using Bookshelf.ViewModels;

namespace Bookshelf.Views;

public partial class CreateUser : ContentPage
{
	public CreateUser(CreateUserVM createUserVM)
	{
		InitializeComponent();

		BindingContext = createUserVM;

    }
}