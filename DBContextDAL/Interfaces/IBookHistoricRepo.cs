using Models.DTOs;

namespace Repositories.Interfaces
{
    public interface IBookHistoricRepo
    {
        Task<int> ExecuteAddBookHistoricAsync(BookHistoric bookHistoric, int uid);

        Task<List<BookHistoric>> GetBookHistoricByBookIdAsync(int uid, int bookId, int page);

        Task<List<BookHistoric>> Get(int uid, int page);
    }
}