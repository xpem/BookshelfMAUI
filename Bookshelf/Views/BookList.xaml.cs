using Bookshelf.ViewModels;

namespace Bookshelf.Views;

public partial class BookList : ContentPage
{
	public BookList(BookListVM bookListVM)
	{
		InitializeComponent();

		BindingContext = bookListVM;
	}
}