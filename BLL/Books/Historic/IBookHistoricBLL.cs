using Models.Books.Historic;

namespace BLL.Books.Historic
{
    public interface IBookHistoricBLL
    {
        Task<BookHistoricList> GetBookHistoricByBookId(int? page, int bookId);
    }
}
