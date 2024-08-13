using BLL.Books;
using BLL.Books.Historic;
using BLL.Books.Historic.Interfaces;
using Bookshelf.Resources.Fonts.Styles;
using Bookshelf.ViewModels.Components;
using Models.Books.Historic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

                List<Models.Books.Historic.BookHistoric> bookHistoricList = await bookHistoricService.GetAsync(((App)App.Current).Uid, pageNumber);

                foreach (Models.Books.Historic.BookHistoric bookHistoricObj in bookHistoricList)
                {
                    StringBuilder bookHistoricText = new();
                    string bookHistoricIcon, updatedFrom, updatedTo;
                    int bookStatusId = 8;
                    bookHistoricText.Append($"<strong>{bookHistoricObj.BookTitle}</strong><br>");
                    if (bookHistoricObj.TypeId == 1)
                    {
                        bookHistoricText.Append($"<br>Livro Adicionado!");
                        bookHistoricIcon = IconFont.Plus;
                    }
                    else
                    {
                        bookHistoricIcon = IconFont.Pen;

                        if (bookHistoricObj.BookHistoricItems.Count > 0)
                        {
                            foreach (Models.Books.Historic.BookHistoricItem bookHistoricItemObj in bookHistoricObj.BookHistoricItems)
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
                        BookTitle = bookHistoricObj.BookTitle
                    };

                    UIBookHistoricList.Add(uIBookHistoric);
                }

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
