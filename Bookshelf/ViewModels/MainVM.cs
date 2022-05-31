using Bookshelf.Utils.Navigation;
using Bookshelf.Views;
using BookshelfServices.Books;
using System.Windows.Input;

namespace Bookshelf.ViewModels
{
    public class MainVM : ViewModelBase
    {
        /// <summary>
        /// definie that the proccess of first sync has executed
        /// </summary>
        private bool firstSync = false;

        private string illRead, reading, read, interrupted, imgSync;

        public string IllRead { get => illRead; set { illRead = value; OnPropertyChanged(); } }

        public string Reading { get => reading; set { reading = value; OnPropertyChanged(); } }

        public string Read { get => read; set { read = value; OnPropertyChanged(); } }

        public string Interrupted { get => interrupted; set { interrupted = value; OnPropertyChanged(); } }

        public string ImgSync { get => imgSync; set { if (value != imgSync) { imgSync = value; OnPropertyChanged(); } } }

        /// <summary>
        /// var that defines if the function that verify the synchronization is running  or not
        /// </summary>
        private static bool ChekingSync { get; set; }

        private double frmMainOpacity;

        public double FrmMainOpacity { get => frmMainOpacity; set { frmMainOpacity = value; OnPropertyChanged(); } }

        private bool frmMainIsEnabled, connectedVisible;

        public bool FrmMainIsEnabled { get => frmMainIsEnabled; set { frmMainIsEnabled = value; OnPropertyChanged(); } }

        public bool ConnectedVisible { get => connectedVisible; set { if (value != connectedVisible) { connectedVisible = value; OnPropertyChanged(); } } }

        readonly IBooksServices booksServices;

        public MainVM(INavigationServices _navigation, IBooksServices _booksServices)
        {
            navigation = _navigation;
            booksServices = _booksServices;
        }

        public ICommand LogoutCommand => new Command(async (e) =>
        {
            bool resp = await Application.Current.MainPage.DisplayAlert("Confirmação", "Deseja sair e retornar a tela inicial?", "Sim", "Cancelar");

            if (resp)
            {
                BookshelfServices.User.UserServices.CleanUserDatabase();

                Application.Current.MainPage = new NavigationPage();
                await navigation.NavigateToPage<Access>();
            }
        });

        /// <summary>
        /// parallel operation that checks if the system is synchronizyng.
        /// </summary>
        private async Task ChekSync()
        {
            ChekingSync = true;

            while (ChekingSync)
            {
                if (!(Connectivity.NetworkAccess == NetworkAccess.Internet))
                {
                    connectedVisible = true;
                }
                else
                {
                    connectedVisible = false;

                    if (BookshelfServices.Books.Sync.BooksSyncServices.Synchronizing)
                    {
                        ImgSync = "rotate_solid_15_green.png";
                    }
                    else
                    {
                        await GetBookshelfTotals();

                        ImgSync = "rotate_solid_15_w.png";

                        //after finish first sync, enable the grid for access
                        if (!firstSync)
                        {
                            FrmMainOpacity = 1;
                            firstSync = true;
                            FrmMainIsEnabled = true;
                        }
                    }
                }

                //in fisrt sync not finish yet, verify with one second interval
                if (!firstSync)
                { await Task.Delay(2000); }
                //checks with ten seconds interval
                else
                { await Task.Delay(10000); }
            }
        }


        public ICommand OnAppearingCommand => new Command((e) =>
        {
            IllRead = Reading = Read = Interrupted = "...";

            if (!ChekingSync)
            {
                FrmMainOpacity = 0.7;
                FrmMainIsEnabled = false;

                _ = ChekSync();
            }
        });

        public async Task GetBookshelfTotals()
        {
            //
            BookshelfModels.Books.Totals totals = await booksServices.GetBookshelfTotals();
            //
            if (totals.IllRead.ToString() != IllRead) { IllRead = totals.IllRead.ToString(); }
            if (totals.Reading.ToString() != Reading) { Reading = totals.Reading.ToString(); }
            if (totals.Read.ToString() != Read) { Read = totals.Read.ToString(); }
            if (totals.Interrupted.ToString() != Interrupted) { Interrupted = totals.Interrupted.ToString(); }
            //
        }



    }
}
