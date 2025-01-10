using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models.Books.Historic;
using Services.Books;
using Services.Books.Historic.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Bookshelf.ViewModels
{
    public partial class HistoricVM(IBookHistoricService bookHistoricService, IBooksOperationService booksOperationService) : ViewModelBase
    {
        public ObservableCollection<UIBookHistoric> UIBookHistoricList { get; } = [];

        public int CurrentPage { get; set; }

        bool isConnected;

        public bool IsConnected
        {
            get => isConnected; set { if (isConnected != value) { SetProperty(ref (isConnected), value); } }
        }

        bool isNotSyncUpdates;

        public bool IsNotSyncUpdates
        {
            get => isNotSyncUpdates; set { if (isNotSyncUpdates != value) { SetProperty(ref (isNotSyncUpdates), value); } }
        }

        Color syncProcessingColor;

        public Color SyncProcessingColor
        {
            get => syncProcessingColor; set
            {
                if (syncProcessingColor != value)
                {
                    syncProcessingColor = value;
                    SetProperty(ref (syncProcessingColor), value);
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
                    SetProperty(ref (syncOptionIsVisible), value);
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
                    SetProperty(ref (syncOptionIsProcessing), value);
                }
            }
        }

        [RelayCommand]
        public Task LoadMore()
        {
            CurrentPage++;
            return LoadListAsync(CurrentPage);
        }

        [RelayCommand]
        public Task Appearing()
        {
            if (UIBookHistoricList.Count > 0)
                UIBookHistoricList.Clear();

            CurrentPage = 1;

            _ = CheckIfHasPendingOperationWithBookId();

            _ = LoadListAsync(CurrentPage);

            return Task.CompletedTask;
        }

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
