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
        UIBookItem TappedItem = e.Item as UIBookItem;
        Shell.Current.GoToAsync($"{nameof(BookDetail)}?Id={TappedItem.Id}", true);
    }

    private async void ViewCell_Tapped(object sender, EventArgs e)
    {
        var cell = sender as ViewCell;
        cell.View.Opacity = 0.5;
        await cell.View.FadeTo(0.5, 2000);
        _ = cell.View.FadeTo(1, 1000).ConfigureAwait(false);
    }
}