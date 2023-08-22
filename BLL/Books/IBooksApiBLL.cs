using Models.Books;
using Models.Responses;

namespace BLL.Books
{
    public interface IBooksApiBLL
    {
         Task<BLLResponse> AddBook(Book book);

        Task<BLLResponse> AltBook(Book book);

        Task<BLLResponse> GetBooksByLastUpdate(DateTime lastUpdate);
    }
}
