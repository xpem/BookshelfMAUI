using BLL.Books.Historic.Interfaces;
using DbContextDAL;
using Models.Books.Historic;

namespace BLL.Books.Historic
{
    public class BookHistoricBLL(IBookHistoricDAL bookHistoricDAL) : IBookHistoricService
    {
        public async Task<List<BookHistoric>> GetByBookIdAsync(int uid, int page, int bookId)
        {
            List<BookHistoric> list = await bookHistoricDAL.GetBookHistoricByBookIdAsync(uid, bookId, page);

            return list;
        }

        public async Task<List<BookHistoric>> GetAsync(int uid, int page)
        {
            try
            {
                List<BookHistoric> list = await bookHistoricDAL.Get(uid, page);
                return list;
            }
            catch (Exception ex) { throw; }

        }
    }
}
