using Bookshelf.ViewModels.GoogleSearch;
using BookshelfModels.Books.GoogleApi;

namespace Bookshelf.Views.GoogleSearch;

public partial class GoogleBooksResults : ContentPage
{
	public GoogleBooksResults(GoogleBooksResultsVM googleBooksResultsVM)
	{
		InitializeComponent();

		BindingContext = googleBooksResultsVM;
	}

    private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        var TappedItem = e.Item as UIGoogleBook;
        Shell.Current.GoToAsync($"{nameof(AddBook)}?GoogleKey={TappedItem.Id}", true);
    }
}