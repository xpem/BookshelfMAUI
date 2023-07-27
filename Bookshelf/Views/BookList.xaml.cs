using Bookshelf.ViewModels;
using Models.Books;

namespace Bookshelf.Views;

public partial class BookList : ContentPage
{
    public BookList(BookListVM bookListVM)
    {
        InitializeComponent();

        BindingContext = bookListVM;
    }

    private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        var TappedItem = e.Item as UIBookItem;
        Shell.Current.GoToAsync($"{nameof(BookDetail)}?Key={TappedItem.Key}", true);
    }
}