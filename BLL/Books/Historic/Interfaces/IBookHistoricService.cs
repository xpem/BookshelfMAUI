using Models.Books.Historic;

namespace BLL.Books.Historic.Interfaces
{
    public interface IBookHistoricService
    {
        Task<List<BookHistoric>> GetByBookIdAsync(int uid, int page, int bookId);
        Task<List<BookHistoric>> GetAsync(int uid, int page);
    }
}
