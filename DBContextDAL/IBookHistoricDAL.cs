using Models.Books.Historic;

namespace DbContextDAL
{
    public interface IBookHistoricDAL
    {
        Task<int> ExecuteAddBookHistoricAsync(BookHistoric bookHistoric, int uid);
        Task<List<BookHistoric>> GetBookHistoricByBookIdAsync(int uid, int bookId,int page);
    }
}