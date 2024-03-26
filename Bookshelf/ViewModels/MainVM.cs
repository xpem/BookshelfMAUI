using BLL;
using BLL.Books;
using BLL.User;
using Bookshelf.Services.Sync;
using Bookshelf.ViewModels.Components;
using Bookshelf.Views;
using Bookshelf.Views.GoogleSearch;
using Models;
using System.Windows.Input;

namespace Bookshelf.ViewModels
{
    public class MainVM(IBookBLL _booksServices, ISyncServices syncService, IBuildDbBLL buildDbBLL) : ViewModelBase
    {
        private bool firstSyncIsRunnig = true;

        private string illRead, reading, read, interrupted;

        private Color isSync;

        public string IllRead { get => illRead; set { if (illRead != value) { illRead = value; OnPropertyChanged(nameof(IllRead)); } } }

        public string Reading { get => reading; set { if (reading != value) { reading = value; OnPropertyChanged(nameof(Reading)); } } }

        public string Read { get => read; set { if (read != value) { read = value; OnPropertyChanged(nameof(Read)); } } }

        public string Interrupted { get => interrupted; set { if (interrupted != value) { interrupted = value; OnPropertyChanged(nameof(Interrupted)); } } }

        public Color IsSync { get => isSync; set { if (value != isSync) { isSync = value; OnPropertyChanged(nameof(IsSync)); } } }

        /// <summary>
        /// var that defines if the function that verify the synchronization is running  or not
        /// </summary>
        private bool ChekingSync { get; set; }

        private double frmMainOpacity;

        public double FrmMainOpacity { get => frmMainOpacity; set { if (frmMainOpacity != value) { frmMainOpacity = value; OnPropertyChanged(nameof(FrmMainOpacity)); } } }

        private bool frmMainIsEnabled;
        Color isConnected;

        public bool FrmMainIsEnabled { get => frmMainIsEnabled; set { if (value != frmMainIsEnabled) { frmMainIsEnabled = value; OnPropertyChanged(nameof(FrmMainIsEnabled)); } } }

        public Color IsConnected { get => isConnected; set { if (value != isConnected) { isConnected = value; OnPropertyChanged(nameof(IsConnected)); } } }

        private Timer _Timer;
        private int Interval = 2000;
        private bool ThreadIsRunning = false;

        public ICommand OnAppearingCommand => new Command((e) =>
        {
            IsSync = Colors.Gray;

            IllRead = Reading = Read = Interrupted = "...";

            Task.Run(GetBookshelfTotalsAsync).Wait();

            SetTimer();
        });

        public void SetTimer()
        {
            if (!ThreadIsRunning)
            {
                FrmMainOpacity = 0.5;

                ThreadIsRunning = true;
                firstSyncIsRunnig = true;
                _Timer = new Timer(CheckSync, null, Interval, Timeout.Infinite);
            }
        }

        /// <summary>
        /// parallel operation that checks if the system is synchronizyng.
        /// </summary>
        private async void CheckSync(object state)
        {
            try
            {
                if (!(Connectivity.NetworkAccess == NetworkAccess.Internet))
                    IsConnected = Colors.Red;
                else
                {
                    IsConnected = Colors.Green;

                    switch (SyncServices.Synchronizing)
                    {
                        case SyncStatus.Processing: IsSync = Colors.Green; break;
                        case SyncStatus.Sleeping:

                            await Task.Run(GetBookshelfTotalsAsync);

                            IsSync = Colors.Gray;

                            //after finish first sync, enable the grid for access
                            if (firstSyncIsRunnig)
                            {
                                FrmMainOpacity = 1;
                                firstSyncIsRunnig = false;
                                FrmMainIsEnabled = false;
                            }
                            else { FrmMainIsEnabled = true; }

                            break;
                        case SyncStatus.ServerOff:
                            IsSync = Colors.Red;
                            break;
                    }
                }

                //in fisrt sync not finish yet, verify with one second interval
                if (firstSyncIsRunnig)
                { Interval = 2000; }
                //checks with ten seconds interval
                else
                { Interval = 10000; }
            }
            finally
            {
                _Timer?.Change(Interval, Timeout.Infinite);
            }
        }

        public async Task GetBookshelfTotalsAsync()
        {
            //
            Models.Books.Totals totals = await _booksServices.GetBookshelfTotalsAsync(((App)Application.Current).Uid);
            //
            if (totals.IllRead.ToString() != IllRead) { IllRead = totals.IllRead.ToString(); }
            if (totals.Reading.ToString() != Reading) { Reading = totals.Reading.ToString(); }
            if (totals.Read.ToString() != Read) { Read = totals.Read.ToString(); }
            if (totals.Interrupted.ToString() != Interrupted) { Interrupted = totals.Interrupted.ToString(); }
            //
        }

        public ICommand GoogleSearchCommand => new Command(async (e) => { await Shell.Current.GoToAsync($"{nameof(GoogleBooksResults)}"); });

        public ICommand ReadCommand => new Command(async (e) => { await CallBookList(3); });

        public ICommand InterruptedCommand => new Command(async (e) => await CallBookList(4));

        public ICommand ReadingCommand => new Command(async (e) => await CallBookList(2));

        public ICommand IllReadCommand => new Command(async (e) => await CallBookList(1));

        public ICommand ArchiveCommand => new Command(async (e) => await CallBookList(0));

        public ICommand LogoutCommand => new Command(async (e) =>
        {
            bool resp = await Application.Current.MainPage.DisplayAlert("Confirmação", "Deseja sair e retornar a tela inicial?", "Sim", "Cancelar");

            if (resp)
            {
                _Timer.Dispose();
                //finalize sync thread process
                syncService.ThreadIsRunning = false;
                ThreadIsRunning = false;

                syncService.Timer?.Dispose();

                ((App)App.Current).Uid = 0;

                await buildDbBLL.CleanLocalDatabase();


                await Shell.Current.GoToAsync($"//{nameof(SignIn)}");
            }
        });

        private async Task CallBookList(int BookSituation) =>
            await Shell.Current.GoToAsync($"{nameof(BookList)}?Situation={BookSituation}", true);

    }
}
