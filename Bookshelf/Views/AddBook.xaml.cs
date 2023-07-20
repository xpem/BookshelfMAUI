using Bookshelf.ViewModels;

namespace Bookshelf.Views;

public partial class AddBook : ContentPage
{
    public AddBook(AddBookVM addBookVM)
    {
        InitializeComponent();

        BindingContext = addBookVM;
    }
}