using Bookshelf.Utils.Navigation;

namespace Bookshelf.ViewModels
{
    public class MainVM : ViewModelBase
    {

        private string illRead, reading, read, interrupted, isSync, isConnected;

        public string IllRead { get => illRead; set { illRead = value; OnPropertyChanged(); } }

        public string Reading { get => reading; set { reading = value; OnPropertyChanged(); } }

        public string Read { get => read; set { read = value; OnPropertyChanged(); } }

        public string Interrupted { get => interrupted; set { interrupted = value; OnPropertyChanged(); } }

        public string IsSync { get => isSync; set { isSync = value; OnPropertyChanged(); } }

        public string IsConnected { get => isConnected; set { isConnected = value; OnPropertyChanged(); } }

        public MainVM(INavigationServices _navigation)
        {
            navigation = _navigation;
        }
    }
}
