using Bookshelf.ViewModels.Book;

namespace Bookshelf.Views.Book;

public partial class BookHistoric : ContentPage
{
    public BookHistoric(BookHistoricVM bookHistoricVM)
    {
        InitializeComponent();
        BindingContext = bookHistoricVM;

    }
}