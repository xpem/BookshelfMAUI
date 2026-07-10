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

    private void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is UIBookItem tappedItem)
        {
            Shell.Current.GoToAsync($"{nameof(BookDetail)}?Id={tappedItem.Id}", true);
        }

        // Reset selection so the same item can be tapped again
        ((CollectionView)sender).SelectedItem = null;
    }
}