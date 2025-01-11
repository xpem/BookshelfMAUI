using Bookshelf.ViewModels;

namespace Bookshelf.Views;

public partial class Main : ContentPage
{
    public Main(MainVM mainVM)
    {
        InitializeComponent();

        BindingContext = mainVM;
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var cell = sender as Border;
        cell.Opacity = 0.5;
        _ = cell.FadeTo(1, 1000);
    }
}