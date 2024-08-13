using Bookshelf.ViewModels;

namespace Bookshelf.Views;

public partial class Historic : ContentPage
{
	public Historic(HistoricVM historicVM)
	{
		InitializeComponent();

		BindingContext = historicVM;
	}
}