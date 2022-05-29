using Bookshelf.Utils.Navigation;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Bookshelf.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public INavigationServices navigation;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool CheckInternet()
        {
            return Connectivity.NetworkAccess == NetworkAccess.Internet;
        }

        public virtual Task OnNavigatingTo(object parameter)
          => Task.CompletedTask;

        public virtual Task OnNavigatedFrom(bool isForwardNavigation)
            => Task.CompletedTask;

        public virtual Task OnNavigatedTo()
            => Task.CompletedTask;
        public virtual void RaisePropertyChanged([CallerMemberName] string property = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

    }
}
