using Bookshelf.Utils.Navigation;

namespace Bookshelf.ViewModels.Components
{
    public class ViewModelBase : BindableObject
    {
        protected INavigationServices navigation;


        bool isBusy;

        public bool IsBusy
        {
            get => isBusy; set { if (isBusy!= value) { isBusy = value; OnPropertyChanged(); } }
        }

        public bool IsNotBusy => !isBusy;

    }
}
