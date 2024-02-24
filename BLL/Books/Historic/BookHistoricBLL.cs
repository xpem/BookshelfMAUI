using BLL.Books.Historic.Interfaces;
using DbContextDAL;
using Models.Books.Historic;

namespace BLL.Books.Historic
{
    public class BookHistoricBLL(IBookHistoricDAL bookHistoricDAL) : IBookHistoricBLL
    {
        public async Task<BookHistoricList> GetBookHistoricByBookIdAsync(int uid, int? page, int bookId)
        {
            int pageSize = 10;

            List<BookHistoric> list = await bookHistoricDAL.GetBookHistoricByBookIdAsync(uid, bookId);

            int total = list.Count;
            if (page != null)
                list = list.Skip((page.Value - 1) * pageSize).Take(pageSize).ToList();

            return new BookHistoricList() { List = list, Total = total };
        }
    }
}
