using BLL.Books.Historic.Interfaces;
using Bookshelf.Resources.Fonts.Styles;
using Bookshelf.UIModels;
using Bookshelf.ViewModels.Components;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace Bookshelf.ViewModels.Book
{
    public class BookHistoricVM : ViewModelBase, IQueryAttributable
    {
        #region Vars

        readonly IBookHistoricBLL BookHistoricBLL;

        private int BookId { get; set; }

        public ObservableCollection<UIBookHistoric> UIBookHistoricList { get; } = new();

        public int CurrentPage { get; set; }
        #endregion

        public BookHistoricVM(IBookHistoricBLL bookHistoricBLL)
        {
            BookHistoricBLL = bookHistoricBLL;
        }


        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            BookId = Convert.ToInt32(query["BookId"]);

            if (UIBookHistoricList.Count > 0)
                UIBookHistoricList.Clear();

            CurrentPage++;
            _ = LoadListAsync(CurrentPage);
        }

        private async Task LoadListAsync(int? pageNumber)
        {
            IsBusy = true;

            Models.Books.Historic.BookHistoricList bookHistoricList = BookHistoricBLL.GetBookHistoricByBookId(pageNumber, BookId);

            foreach (Models.Books.Historic.BookHistoric bookHistoricObj in bookHistoricList.List)
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
            _ = LoadListAsync(CurrentPage);
        });

        private static string BuildStatusText(int statusId)
        {
            return statusId switch
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
}
