using Bookshelf.ViewModels.Book;

namespace Bookshelf.Views;

public partial class BookDetail : ContentPage
{
    public BookDetail(BookDetailVM bookDetailVM)
    {
        InitializeComponent();
        BindingContext = bookDetailVM;
    }
}