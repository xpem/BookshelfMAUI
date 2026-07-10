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

    private void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is UIGoogleBook tappedItem)
        {
            Shell.Current.GoToAsync($"{nameof(AddBook)}", true, new Dictionary<string, object> { { "GoogleBook", tappedItem } });
        }

        // Reset selection so the same item can be tapped again
        ((CollectionView)sender).SelectedItem = null;
    }
}