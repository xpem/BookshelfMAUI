using BLL.Books;
using BLL.Books.Historic.Interfaces;
using Bookshelf.Resources.Fonts.Styles;
using Bookshelf.Services.Sync;
using Bookshelf.ViewModels.Components;
using DbContextDAL;
using Models.Books.Historic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace Bookshelf.ViewModels.Book
{
    public class BookHistoricVM(IBookHistoricService bookHistoricBLL, IBooksOperationService booksOperationBLL, ISyncServices syncServices) : ViewModelBase, IQueryAttributable
    {

        #region Vars

        private int BookId { get; set; }

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

        //Color.FromArgb("#a3e4d7")
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

        #endregion

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            BookId = Convert.ToInt32(query["BookId"]);
        }

        public ICommand OnAppearingCommand => new Command((e) =>
        {
            if (UIBookHistoricList.Count > 0)
                UIBookHistoricList.Clear();

            CurrentPage = 1;

            _ = CheckIfHasPendingOperationWithBookId();

            _ = LoadListAsync(CurrentPage);
        });


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

        public ICommand SyncCommand => new Command(async (e) =>
        {
            SyncOptionIsProcessing = true;
            SyncProcessingColor = Color.FromArgb("#F8D210");

            await syncServices.ExecSyncAsync();

            SyncOptionIsProcessing = false;
            SyncProcessingColor = Color.FromArgb("#919191");

            _ = CheckIfHasPendingOperationWithBookId();
        });

        private async Task LoadListAsync(int pageNumber)
        {
            IsBusy = true;

            List<Models.Books.Historic.BookHistoric> bookHistoricList = await bookHistoricBLL.GetByBookIdAsync(((App)App.Current).Uid, pageNumber, BookId);

            foreach (Models.Books.Historic.BookHistoric bookHistoricObj in bookHistoricList)
            {
                StringBuilder bookHistoricText = new();
                string bookHistoricIcon, updatedFrom, updatedTo;
                int bookStatusId = 8;

                if (bookHistoricObj.TypeId == 1)
                {
                    bookHistoricText.Append($"<strong>Livro Adicionado!</strong>");
                    bookHistoricIcon = IconFont.Plus;
                }
                else
                {
                    bookHistoricIcon = IconFont.Pen;

                    if (bookHistoricObj.BookHistoricItems.Count > 0)
                    {
                        foreach (BookHistoricItem bookHistoricItemObj in bookHistoricObj.BookHistoricItems)
                        {
                            if (bookHistoricText.Length > 0) bookHistoricText.Append("<br>");

                            if (bookHistoricItemObj.BookFieldId == bookStatusId)
                            {
                                if (!string.IsNullOrEmpty(bookHistoricItemObj.UpdatedFrom))
                                    updatedFrom = $"de '{BookHistoricItem.BuildStatusText(Convert.ToInt32(bookHistoricItemObj.UpdatedFrom))}'";
                                else updatedFrom = "";

                                updatedTo = $"'{BookHistoricItem.BuildStatusText(Convert.ToInt32(bookHistoricItemObj.UpdatedTo))}'";
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(bookHistoricItemObj.UpdatedFrom))
                                    updatedFrom = $"de '{bookHistoricItemObj.UpdatedFrom}'";
                                else updatedFrom = "";

                                updatedTo = $"'{bookHistoricItemObj.UpdatedTo}'";
                            }

                            bookHistoricText.Append($"<strong>{bookHistoricItemObj.BookFieldName}</strong>: {updatedFrom} para {updatedTo};");
                        }
                    }
                }

                UIBookHistoric uIBookHistoric = new()
                {
                    Id = bookHistoricObj.Id.Value,
                    HistoricDate = string.Format("Em {0:dd/MM/yyyy} as {0:H:mm}", bookHistoricObj.CreatedAt),
                    BookHistoricIcon = bookHistoricIcon,
                    BookHistoricText = bookHistoricText.ToString(),
                };

                UIBookHistoricList.Add(uIBookHistoric);
            }

            IsBusy = false;
        }

        public ICommand LoadMoreCommand => new Command(() =>
        {
            CurrentPage++;
            Task.Run(() => LoadListAsync(CurrentPage));
        });
    }
}
