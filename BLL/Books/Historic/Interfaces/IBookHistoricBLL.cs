using Models.Books.Historic;

namespace BLL.Books.Historic.Interfaces
{
    public interface IBookHistoricBLL
    {
        Task<BookHistoricList> GetBookHistoricByBookId(int? page, int bookId);
    }
}
