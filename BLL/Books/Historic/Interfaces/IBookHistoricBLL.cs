using Models.Books.Historic;

namespace BLL.Books.Historic.Interfaces
{
    public interface IBookHistoricBLL
    {
        BookHistoricList GetBookHistoricByBookId(int? page, int bookId);
    }
}
