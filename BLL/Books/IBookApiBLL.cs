using Models.Books;
using Models.Responses;

namespace BLL.Books
{
    public interface IBookApiBLL
    {
        Task<BLLResponse> AddBook(Book book);

        Task<BLLResponse> UpdateBook(Book book);

        Task<BLLResponse> GetBooksByLastUpdate(DateTime lastUpdate);
    }
}
