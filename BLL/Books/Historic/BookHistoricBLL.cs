using Repositories.Interfaces;
using Models;
using Models.Books.Historic;
using System.Text;
using Models.DTOs;
using Services.Books.Historic.Interfaces;

namespace Services.Books.Historic
{
    public class BookHistoricBLL(IBookHistoricRepo bookHistoricDAL) : IBookHistoricService
    {
        public async Task<List<UIBookHistoric>> GetByBookIdAsync(int uid, int page, int bookId)
        {
            List<UIBookHistoric> uIBookHistoricList = [];

            List<BookHistoric> list = await bookHistoricDAL.GetBookHistoricByBookIdAsync(uid, bookId, page);

            foreach (BookHistoric bookHistoricObj in list)
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

                uIBookHistoricList.Add(uIBookHistoric);
            }

            return uIBookHistoricList;
        }

        public async Task<List<UIBookHistoric>> GetAsync(int uid, int page)
        {
            List<UIBookHistoric> uIBookHistoricList = [];
            try
            {
                List<BookHistoric> bookHistoricList = await bookHistoricDAL.Get(uid, page);

                foreach (BookHistoric bookHistoricObj in bookHistoricList)
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

                        if (bookHistoricObj.BookHistoricItems?.Count > 0)
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
                        BookTitle = bookHistoricObj.BookTitle
                    };

                    uIBookHistoricList.Add(uIBookHistoric);
                }

                return uIBookHistoricList;
            }
            catch (Exception ex) { throw; }

        }
    }
}
