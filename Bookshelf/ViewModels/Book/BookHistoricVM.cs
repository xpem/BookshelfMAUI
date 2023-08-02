using BLL.Books.Historic.Interfaces;
using Bookshelf.Resources.Fonts.Styles;
using Bookshelf.UIModels;
using Bookshelf.ViewModels.Components;
using Models.Books;
using Models.Books.Historic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

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

            if (UIBookHistoricList.Count > 0)
                UIBookHistoricList.Clear();

            CurrentPage++;
            LoadList(CurrentPage);
        }

        private async void LoadList(int? pageNumber)
        {
            IsBusy = true;

            Models.Books.Historic.BookHistoricList bookHistoricList = await BookHistoricBLL.GetBookHistoricByBookId(pageNumber, 216);

            foreach (var bookHistoricObj in bookHistoricList.List)
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
                    bookHistoricText.Append($"<strong>Alteração</strong><br>");


                    if (bookHistoricObj.BookHistoricItems.Count > 0)
                    {
                        foreach (var bookHistoricItemObj in bookHistoricObj.BookHistoricItems)
                        {
                            if (bookHistoricItemObj.BookFieldId == bookStatusId)
                            {
                                updatedFrom = BuildStatusText(Convert.ToInt32(bookHistoricItemObj.UpdatedFrom));
                                updatedTo = BuildStatusText(Convert.ToInt32(bookHistoricItemObj.UpdatedTo));
                            }
                            else
                            {
                                updatedFrom = bookHistoricItemObj.UpdatedFrom;
                                updatedTo = bookHistoricItemObj.UpdatedTo;
                            }

                            bookHistoricText.Append($"<br> <strong>{bookHistoricItemObj.BookFieldName}</strong>: de {updatedFrom} para {updatedTo};");
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
            LoadList(CurrentPage);
        });


        private static string BuildStatusText(int statusId)
        {
            return statusId switch
            {
                0 => "'Nenhum'",
                1 => "'Vou ler'",
                2 => "'Lendo'",
                3 => "'Lido'",
                4 => "'Interrompido'",
                _ => "Desconhecido",
            };
        }

    }
}
