using BLL.Books;
using BLL.Books.Historic.Interfaces;
using Bookshelf.ViewModels.Components;
using Models.Books.Historic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Bookshelf.ViewModels
{
    public class HistoricVM(IBookHistoricService bookHistoricService, IBooksOperationService booksOperationService) : ViewModelBase
    {
        public ObservableCollection<UIBookHistoric> UIBookHistoricList { get; } = [];

        public int CurrentPage { get; set; }

        bool isConnected;

        public bool IsConnected
        {
            get => isConnected; set { if (isConnected != value) { isConnected = value; OnPropertyChanged(nameof(IsConnected)); } }
        }

        bool isNotSyncUpdates;

        public bool IsNotSyncUpdates
        {
            get => isNotSyncUpdates; set { if (isNotSyncUpdates != value) { isNotSyncUpdates = value; OnPropertyChanged(nameof(IsNotSyncUpdates)); } }
        }

        Color syncProcessingColor;

        public Color SyncProcessingColor
        {
            get => syncProcessingColor; set
            {
                if (syncProcessingColor != value)
                {
                    syncProcessingColor = value;
                    OnPropertyChanged(nameof(SyncProcessingColor));
                }
            }
        }

        bool syncOptionIsVisible;

        public bool SyncOptionIsVisible
        {
            get => syncOptionIsVisible; set
            {
                if (syncOptionIsVisible != value)
                {
                    syncOptionIsVisible = value;
                    OnPropertyChanged(nameof(SyncOptionIsVisible));
                }
            }
        }

        bool syncOptionIsProcessing;

        public bool SyncOptionIsProcessing
        {
            get => syncOptionIsProcessing; set
            {
                if (syncOptionIsProcessing != value)
                {
                    syncOptionIsProcessing = value; OnPropertyChanged(nameof(SyncOptionIsProcessing));
                }
            }
        }

        public ICommand LoadMoreCommand => new Command(() =>
        {
            CurrentPage++;
            Task.Run(() => LoadListAsync(CurrentPage));
        });

        public ICommand OnAppearingCommand => new Command((e) =>
        {
            if (UIBookHistoricList.Count > 0)
                UIBookHistoricList.Clear();

            CurrentPage = 1;

            _ = CheckIfHasPendingOperationWithBookId();

            _ = LoadListAsync(CurrentPage);
        });

        private async Task LoadListAsync(int pageNumber)
        {
            try
            {
                IsBusy = true;

                List<Models.Books.Historic.UIBookHistoric> bookHistoricList = await bookHistoricService.GetAsync(((App)App.Current).Uid, pageNumber);

                if (bookHistoricList.Count > 0)
                    foreach (var item in bookHistoricList)
                        UIBookHistoricList.Add(item);

                IsBusy = false;
            }
            catch (Exception ex) { throw ex; }
        }

        public async Task CheckIfHasPendingOperationWithBookId()
        {
            if (await booksOperationService.CheckIfHasPendingOperation())
            {
                IsNotSyncUpdates = true;
                if (IsOn)
                    SyncOptionIsVisible = true;
            }
            else
            {
                IsNotSyncUpdates = false;
                SyncOptionIsVisible = false;
            }
        }
    }
}
