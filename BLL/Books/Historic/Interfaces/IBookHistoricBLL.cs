using Models.Books.Historic;

namespace BLL.Books.Historic.Interfaces
{
    public interface IBookHistoricBLL
    {
        Task<BookHistoricList> GetBookHistoricByBookIdAsync(int uid, int? page, int bookId);
    }
}
