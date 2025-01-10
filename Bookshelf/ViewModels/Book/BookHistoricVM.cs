using Bookshelf.Services.Sync;
using CommunityToolkit.Mvvm.Input;
using Models.Books.Historic;
using Services.Books;
using Services.Books.Historic.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Bookshelf.ViewModels.Book
{
    public partial class BookHistoricVM(IBookHistoricService bookHistoricBLL, IBooksOperationService booksOperationBLL, ISyncService syncServices) : ViewModelBase, IQueryAttributable
    {

        #region Vars

        private int BookId { get; set; }

        private ObservableCollection<UIBookHistoric> uIBookHistoricList = [];

        public ObservableCollection<UIBookHistoric> UIBookHistoricList
        {
            get => uIBookHistoricList;
            set
            {
                if (uIBookHistoricList != value)
                {
                    SetProperty(ref (uIBookHistoricList), value);
                }
            }
        }

        public int CurrentPage { get; set; }

        bool isConnected;

        public bool IsConnected
        {
            get => isConnected; set { if (isConnected != value) { SetProperty(ref (isConnected), value); } }
        }

        bool isNotSyncUpdates;

        public bool IsNotSyncUpdates
        {
            get => isNotSyncUpdates; set
            {
                if (isNotSyncUpdates != value)
                {
                    SetProperty(ref (isNotSyncUpdates), value);
                }
            }
        }

        //Color.FromArgb("#a3e4d7")
        Color syncProcessingColor;

        public Color SyncProcessingColor
        {
            get => syncProcessingColor; set
            {
                if (syncProcessingColor != value)
                {
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

        #endregion

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            BookId = Convert.ToInt32(query["BookId"]);
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


        public async Task CheckIfHasPendingOperationWithBookId()
        {
            if (await booksOperationBLL.CheckIfHasPendingOperationsWithBookId(BookId))
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

        [RelayCommand]
        public async Task Sync()
        {
            SyncOptionIsProcessing = true;
            SyncProcessingColor = Color.FromArgb("#F8D210");

            await syncServices.ExecSyncAsync();

            SyncOptionIsProcessing = false;
            SyncProcessingColor = Color.FromArgb("#919191");

            _ = CheckIfHasPendingOperationWithBookId();
        }

        private async Task LoadListAsync(int pageNumber)
        {
            IsBusy = true;

            List<Models.Books.Historic.UIBookHistoric> bookHistoricList = await bookHistoricBLL.GetByBookIdAsync(((App)App.Current).Uid, pageNumber, BookId);

            if (bookHistoricList.Count > 0)
                foreach (var item in bookHistoricList)
                    UIBookHistoricList.Add(item);

            IsBusy = false;
        }

        [RelayCommand]
        public Task LoadMore()
        {
            CurrentPage++;
            _ = LoadListAsync(CurrentPage);

            return Task.CompletedTask;
        }
    }
}
