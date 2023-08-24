namespace Bookshelf.ViewModels.Components
{
    public class ViewModelBase : BindableObject
    {

        bool isBusy;

        public bool IsBusy
        {
            get => isBusy; set { if (isBusy!= value) { isBusy = value; OnPropertyChanged(nameof(IsBusy)); } }
        }

        public bool IsNotBusy => !isBusy;

    }
}
