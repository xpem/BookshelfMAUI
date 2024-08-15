using BLL.Books.Historic.Interfaces;
using Repositories.Interfaces;
using Models.Books.Historic;
using Models.Exceptions;

namespace BLL.Books.Historic.Sync
{
    public class BookHistoricSyncBLL(IBookHistoricApiServices bookHistoricApiBLL, IBookHistoricDAL bookHistoricDAL) : IBookHistoricSyncBLL
    {
        private const int PAGEMAX = 50;

        public async Task ApiToLocalSync(int uid, DateTime lastUpdate)
        {
            int page = 1;

            while (true)
            {
                Models.Responses.BLLResponse respGetBookHistoricListByCreatedAt = await bookHistoricApiBLL.GetByLastCreatedAtAsync(lastUpdate, page);

                if (respGetBookHistoricListByCreatedAt.Success && respGetBookHistoricListByCreatedAt.Content is not null)
                {
                    List<BookHistoric>? bookHistoricsList = respGetBookHistoricListByCreatedAt.Content as List<BookHistoric>;

                    if (bookHistoricsList is not null)
                    {
                        foreach (BookHistoric bookHistoric in bookHistoricsList)
                        {
                            await bookHistoricDAL.ExecuteAddBookHistoricAsync(bookHistoric, uid);
                        }

                        if (bookHistoricsList.Count < PAGEMAX)
                            break;
                    }
                    else break;
                }
                else throw new BookshelfAPIException("Erro ao tentar utilizar a api do UniqueServer/bookshelf/bookHistorics: " + respGetBookHistoricListByCreatedAt?.ErrorMessage);

                page++;
            }
        }
    }
}
