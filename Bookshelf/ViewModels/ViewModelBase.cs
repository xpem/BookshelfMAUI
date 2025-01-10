using CommunityToolkit.Mvvm.ComponentModel;

namespace Bookshelf.ViewModels
{
    public partial class ViewModelBase : ObservableObject
    {

        bool isBusy;

        public bool IsBusy
        {
            get => isBusy; set { if (isBusy != value) { isBusy = value; SetProperty(ref (isBusy), value); } }
        }

        public bool IsNotBusy => !isBusy;

        protected static bool IsOn => Connectivity.NetworkAccess == NetworkAccess.Internet;
    }
}
