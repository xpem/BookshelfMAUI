using Bookshelf.ViewModels.GoogleSearch;

namespace Bookshelf.Views.GoogleSearch;

public partial class GoogleBooksResults : ContentPage
{
	public GoogleBooksResults(GoogleBooksResultsVM googleBooksResultsVM)
	{
		InitializeComponent();

		BindingContext = googleBooksResultsVM;
	}
}