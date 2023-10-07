using Models.Books.Historic;

namespace DbContextDAL
{
    public interface IBookHistoricDAL
    {
        Task<int> ExecuteAddBookHistoricAsync(BookHistoric bookHistoric, int uid);
        List<BookHistoric> GetBookHistoricByBookId(int uid, int bookId);
    }
}