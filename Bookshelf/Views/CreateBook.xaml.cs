using Bookshelf.ViewModels;

namespace Bookshelf.Views;

public partial class CreateBook : ContentPage
{
    public CreateBook(CreateBookVM createBookVM)
    {
        InitializeComponent();

        BindingContext = createBookVM;

    }
}