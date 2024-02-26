using BLL.Books.Historic.Interfaces;
using DbContextDAL;
using Models.Books.Historic;

namespace BLL.Books.Historic
{
    public class BookHistoricBLL(IBookHistoricDAL bookHistoricDAL) : IBookHistoricBLL
    {
        public async Task<List<BookHistoric>> GetBookHistoricByBookIdAsync(int uid, int page, int bookId)
        {
            List<BookHistoric> list = await bookHistoricDAL.GetBookHistoricByBookIdAsync(uid, bookId, page);

            return list;
        }
    }
}
