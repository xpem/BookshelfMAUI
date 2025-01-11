using Bookshelf.Services.Sync;
using Bookshelf.Views;
using Bookshelf.Views.GoogleSearch;
using CommunityToolkit.Mvvm.Input;
using Models;
using Models.DTOs;
using Services.Books.Interfaces;
using System.Windows.Input;

namespace Bookshelf.ViewModels
{
    public partial class MainVM(IBookService _booksServices, ISyncService syncService) : ViewModelBase
    {
        private bool firstSyncIsRunnig = true;

        private string illRead, reading, read, interrupted;

        private string version = ((App)Application.Current).Version;

        private Color isSync, isConnected;

        public string Version { get => version; set { if (version != value) { SetProperty(ref (version), value); } } }

        public string IllRead { get => illRead; set { if (illRead != value) { SetProperty(ref (illRead), value); } } }

        public string Reading { get => reading; set { if (reading != value) { SetProperty(ref (reading), value); } } }

        public string Read { get => read; set { if (read != value) { SetProperty(ref (read), value); } } }

        public string Interrupted { get => interrupted; set { if (interrupted != value) { SetProperty(ref (interrupted), value); } } }

        public Color IsSync { get => isSync; set { if (value != isSync) { SetProperty(ref (isSync), value); } } }

        /// <summary>
        /// var that defines if the function that verify the synchronization is running  or not
        /// </summary>
        private bool ChekingSync { get; set; }

        private double frmMainOpacity;

        public double FrmMainOpacity { get => frmMainOpacity; set { if (frmMainOpacity != value) { frmMainOpacity = value; OnPropertyChanged(nameof(FrmMainOpacity)); } } }

        private bool frmMainIsEnabled;

        public bool FrmMainIsEnabled { get => frmMainIsEnabled; set { if (value != frmMainIsEnabled) { frmMainIsEnabled = value; OnPropertyChanged(nameof(FrmMainIsEnabled)); } } }

        public Color IsConnected { get => isConnected; set { if (value != isConnected) { isConnected = value; OnPropertyChanged(nameof(IsConnected)); } } }

        private static Timer _Timer;

        private int Interval = 3000;

        [RelayCommand]
        private async Task Appearing()
        {
            if (((App)Application.Current).Uid is null)
            {
                _ = Shell.Current.GoToAsync($"{nameof(SignIn)}");
            }
            else
            {
                IsSync = Colors.Gray;

                IllRead = Reading = Read = Interrupted = "...";

                await GetBookshelfTotalsAsync();

                SetTimer();
            }
        }

        private void SetTimer()
        {
            if (_Timer == null)
            {
                FrmMainOpacity = 0.5;
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

                    switch (SyncService.Synchronizing)
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
                        case SyncStatus.Unauthorized:
                            MainThread.BeginInvokeOnMainThread(() => { ReSignAsync(); });

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

        private async Task ReSignAsync()
        {
            //finalize sync thread process
            syncService.ThreadIsRunning = false;

            syncService.Timer?.Dispose();

            await GoToAsync($"//{nameof(SignIn)}");
        }

        public async Task GetBookshelfTotalsAsync()
        {
            //
            Models.Books.Totals totals = await _booksServices.GetBookshelfTotalsAsync(((App)Application.Current).Uid.Value);
            //
            if (totals.IllRead.ToString() != IllRead) { IllRead = totals.IllRead.ToString(); }
            if (totals.Reading.ToString() != Reading) { Reading = totals.Reading.ToString(); }
            if (totals.Read.ToString() != Read) { Read = totals.Read.ToString(); }
            if (totals.Interrupted.ToString() != Interrupted) { Interrupted = totals.Interrupted.ToString(); }
            //
        }

        [RelayCommand]
        private Task GoogleSearch() => Shell.Current.GoToAsync($"{nameof(GoogleBooksResults)}");

        [RelayCommand]
        private Task ListRead() => CallBookList(3);

        [RelayCommand]
        private Task ListInterrupted() => CallBookList(4);

        [RelayCommand]
        private Task ListReading() => CallBookList(2);

        [RelayCommand]
        private Task ListIllRead() => CallBookList(1);

        //public ICommand ArchiveCommand => new Command((e) => _ = CallBookList(0));

        private Task CallBookList(int BookSituation) => GoToAsync($"{nameof(BookList)}?Situation={BookSituation}");// await Shell.Current.GoToAsync($"{nameof(BookList)}?Situation={BookSituation}", true);

        private static Task GoToAsync(string state)
        {
            _Timer = null;
            //_Timer.Dispose();
            return Shell.Current.GoToAsync(state, true);
        }
    }
}
