using Bookshelf.ViewModels.Book;

namespace Bookshelf.Views;

public partial class AddBook : ContentPage
{
    public AddBook(AddBookVM addBookVM)
    {
        InitializeComponent();

        BindingContext = addBookVM;
    }
}