using Bookshelf.ViewModels;

namespace Bookshelf.Views;

public partial class FirstSyncProcess : ContentPage
{
    public FirstSyncProcess(FirstSyncProcessVM firstSyncProcessVM)
    {
        InitializeComponent();

        BindingContext = firstSyncProcessVM;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is FirstSyncProcessVM vm)
        {
            _ = vm.SynchronizingProcess();
        }
    }
}