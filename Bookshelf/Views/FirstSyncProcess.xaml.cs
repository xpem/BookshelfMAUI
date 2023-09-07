using Bookshelf.ViewModels;

namespace Bookshelf.Views;

public partial class FirstSyncProcess : ContentPage
{
    public FirstSyncProcess(FirstSyncProcessVM firstSyncProcessVM)
    {
        InitializeComponent();

        BindingContext = firstSyncProcessVM;
    }
}