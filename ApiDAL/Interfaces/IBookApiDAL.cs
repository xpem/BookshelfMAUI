using Models.Books;
using Models.Responses;

namespace ApiDAL.Interfaces
{
    public interface IBookApiDAL
    {
        Task<ApiResponse> AddBook(Book book);

        Task<ApiResponse> UpdateBook(Book book);

        Task<ApiResponse> GetBooksByLastUpdate(DateTime lastUpdate);
    }
}
