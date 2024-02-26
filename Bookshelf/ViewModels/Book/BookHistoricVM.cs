using BLL.Books.Historic.Interfaces;
using Bookshelf.Resources.Fonts.Styles;
using Bookshelf.UIModels;
using Bookshelf.ViewModels.Components;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace Bookshelf.ViewModels.Book
{
    public class BookHistoricVM(IBookHistoricBLL bookHistoricBLL) : ViewModelBase, IQueryAttributable
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

        #endregion

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            BookId = Convert.ToInt32(query["BookId"]);

            //if (UIBookHistoricList.Count > 0)
            //    UIBookHistoricList.Clear();

            //CurrentPage++;

            //Task.Run(() => LoadListAsync(CurrentPage));
        }
        public ICommand OnAppearingCommand => new Command((e) =>
        {
            if (UIBookHistoricList.Count > 0)
                UIBookHistoricList.Clear();

            CurrentPage = 1;

            _ = LoadListAsync(CurrentPage);
        });

        private async Task LoadListAsync(int pageNumber)
        {
            IsBusy = true;

            List<Models.Books.Historic.BookHistoric> bookHistoricList = await bookHistoricBLL.GetBookHistoricByBookIdAsync(((App)App.Current).Uid, pageNumber, BookId);

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
                        foreach (Models.Books.Historic.BookHistoricItem bookHistoricItemObj in bookHistoricObj.BookHistoricItems)
                        {
                            if (bookHistoricText.Length > 0) bookHistoricText.Append("<br>");

                            if (bookHistoricItemObj.BookFieldId == bookStatusId)
                            {
                                if (!string.IsNullOrEmpty(bookHistoricItemObj.UpdatedFrom))
                                    updatedFrom = $"de '{BuildStatusText(Convert.ToInt32(bookHistoricItemObj.UpdatedFrom))}'";
                                else updatedFrom = "";

                                updatedTo = $"'{BuildStatusText(Convert.ToInt32(bookHistoricItemObj.UpdatedTo))}'";
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

        private static string BuildStatusText(int statusId) =>
            statusId switch
            {
                0 => "Nenhum",
                1 => "Vou ler",
                2 => "Lendo",
                3 => "Lido",
                4 => "Interrompido",
                _ => "Desconhecido",
            };
    }
}
