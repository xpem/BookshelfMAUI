using Models.Books;
using Models.Responses;

namespace BLL.Books
{
    public interface IBookApiBLL
    {
        Task<BLLResponse> AddBookAsync(Book book);

        Task<BLLResponse> UpdateBookAsync(Book book);

        Task<BLLResponse> GetBooksByLastUpdateAsync(DateTime lastUpdate);
    }
}
