using Bookshelf.Utils.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bookshelf.ViewModels.Components
{
    public partial class ViewModelBase : ObservableObject
    {
        protected INavigationServices navigation;

        [ObservableProperty]
        [AlsoNotifyChangeFor(nameof(IsNotBusy))]
        bool isBusy;

        public bool IsNotBusy => isBusy;
    }
}
