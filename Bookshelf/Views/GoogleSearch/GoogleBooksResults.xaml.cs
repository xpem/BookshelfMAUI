using Bookshelf.ViewModels.GoogleSearch;
using Models.Books.GoogleApi;

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
        UIGoogleBook TappedItem = e.Item as UIGoogleBook;
        Shell.Current.GoToAsync($"{nameof(AddBook)}", true, new Dictionary<string, object> { { "GoogleBook", TappedItem } });
    }
}