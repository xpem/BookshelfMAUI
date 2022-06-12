using Bookshelf.Utils.Navigation;
using Bookshelf.ViewModels.Components;
using Bookshelf.Views;
using BookshelfServices.Books;
using BookshelfServices.Books.Sync;
using Microsoft.Maui.Controls;
using System.Windows.Input;

namespace Bookshelf.ViewModels
{
    public class MainVM : ViewModelBase
    {
        private bool firstSyncIsRunnig = true;

        private string illRead, reading, read, interrupted, imgSync;

        public string IllRead { get => illRead; set { if (illRead != value) { illRead = value; OnPropertyChanged(); } } }

        public string Reading { get => reading; set { if (reading != value) { reading = value; OnPropertyChanged(); } } }

        public string Read { get => read; set { if (read != value) { read = value; OnPropertyChanged(); } } }

        public string Interrupted { get => interrupted; set { if (interrupted != value) { interrupted = value; OnPropertyChanged(); } } }

        public string ImgSync { get => imgSync; set { if (value != imgSync) { imgSync = value; OnPropertyChanged(); } } }

        /// <summary>
        /// var that defines if the function that verify the synchronization is running  or not
        /// </summary>
        private bool ChekingSync { get; set; }

        private double frmMainOpacity;

        public double FrmMainOpacity { get => frmMainOpacity; set { if (frmMainOpacity != value) { frmMainOpacity = value; OnPropertyChanged(); } } }

        private bool frmMainIsEnabled, connectedVisible;

        public bool FrmMainIsEnabled { get => frmMainIsEnabled; set { if (value != frmMainIsEnabled) { frmMainIsEnabled = value; OnPropertyChanged(); } } }

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

                //finalize sync thread process
                BooksSyncServices.ThreadIsRunning = false;

                await Shell.Current.GoToAsync($"//{nameof(Login)}");
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
                        if (firstSyncIsRunnig)
                        {
                            FrmMainOpacity = 1;
                            firstSyncIsRunnig = false;
                            FrmMainIsEnabled = false;
                        }
                        else { FrmMainIsEnabled = true; }
                    }
                }

                //in fisrt sync not finish yet, verify with one second interval
                if (firstSyncIsRunnig)
                { await Task.Delay(2000); }
                //checks with ten seconds interval
                else
                { await Task.Delay(10000); }
            }
        }

        public ICommand OnAppearingCommand => new Command((e) =>
        {
            IllRead = Reading = Read = Interrupted = "...";
            FrmMainOpacity = 0.5;
            _ = ChekSync();

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

        public ICommand CreateBookCommand => new Command(async (e) =>
        {
            var route = $"{nameof(AddBook)}";
            await Shell.Current.GoToAsync(route);


            //Page page = navigation.ResolvePage<AddBook>();
            //(page?.BindingContext as AddBookVM).OnNavigatingTo("");
            //await (Application.Current?.MainPage?.Navigation).PushAsync(page, true);
        });

        public ICommand ReadCommand => new Command(async (e) => { if (FrmMainIsEnabled) await CallBookList(3); });

        public ICommand InterruptedCommand => new Command(async (e) => await CallBookList(4));

        public ICommand ReadingCommand => new Command(async (e) => await CallBookList(2));

        public ICommand IllReadCommand => new Command(async (e) => await CallBookList(1));

        public ICommand ArchiveCommand => new Command(async (e) => await CallBookList(0));

        private async Task CallBookList(int BookSituation)
        {
            //define the page
            Page page = navigation.ResolvePage<BookList>();

            //pass parameter
            (page?.BindingContext as BookListVM).OnNavigatingTo(BookSituation);

            //push ui
            var route = $"{nameof(BookList)}";
            await Shell.Current.GoToAsync(route);
        }
    }
}
