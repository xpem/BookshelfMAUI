using Bookshelf.ViewModels;

namespace Bookshelf.Views;

public partial class BookDetail : ContentPage
{
    public BookDetail(BookDetailVM bookDetailVM)
    {
        InitializeComponent();
        BindingContext = bookDetailVM;
    }
}