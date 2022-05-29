using Bookshelf.ViewModels;

namespace Bookshelf.Views;

public partial class Access : ContentPage
{
    public Access(AccessVM accessVM)
    {
        InitializeComponent();

        BindingContext = accessVM;
    }
}