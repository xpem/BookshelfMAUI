using Bookshelf.ViewModels;

namespace Bookshelf.Views;

public partial class Main : ContentPage
{


    public Main(MainVM mainVM)
    {
        InitializeComponent();

        BindingContext = mainVM;
    }
}