using BLL.Books.Historic;
using Bookshelf.Resources.Fonts.Styles;
using Bookshelf.UIModels;
using Bookshelf.ViewModels.Components;
using Models.Books;
using Models.Books.Historic;
using System.Collections.ObjectModel;
using System.Text;

namespace Bookshelf.ViewModels.Book
{
    public class BookHistoricVM : ViewModelBase, IQueryAttributable
    {
        #region Vars

        readonly IBookHistoricBLL BookHistoricBLL;

        public ObservableCollection<UIBookHistoric> UIBookHistoricList { get; } = new();

        public int CurrentPage { get; set; }
        #endregion

        public BookHistoricVM(IBookHistoricBLL bookHistoricBLL)
        {
            BookHistoricBLL = bookHistoricBLL;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            //    SituationIndex = Convert.ToInt16(query["Situation"].ToString());

            //if (BooksList.Count > 0)
            //    BooksList.Clear();

            CurrentPage++;
            LoadList(CurrentPage);
        }

        private async void LoadList(int? pageNumber)
        {
            //PageTitle = "Carregando lista...";
            IsBusy = true;

            Models.Books.Historic.BookHistoricList bookHistoricList = await BookHistoricBLL.GetBookHistoricByBookId(pageNumber, 209);

            foreach (var bookHistoricObj in bookHistoricList.List)
            {
                StringBuilder bookHistoricText = new();

                bookHistoricText.Append($"<strong>{bookHistoricObj.Type}</strong>");


                if(bookHistoricObj.BookHistoricItems.Count > 0)
                {
                    foreach(var bookHistoricItemObj in bookHistoricObj.BookHistoricItems)
                    {
                        bookHistoricText.Append($"<br> <strong>{bookHistoricItemObj.BookFieldName}</strong>: de {bookHistoricItemObj.UpdatedFrom} para {bookHistoricItemObj.UpdatedTo}");
                    }
                }

                UIBookHistoric uIBookHistoric = new()
                {
                    Id = bookHistoricObj.Id.Value,
                    HistoricDate = string.Format("Em {0:dd/MM/yyyy} as {0:hh:mm}", bookHistoricObj.CreatedAt),
                    BookHistoricIcon = IconFont.Plus,
                    BookHistoricText = bookHistoricText.ToString(),
                };
                UIBookHistoricList.Add(uIBookHistoric);
            }

            IsBusy = false;
        }

    }
}
