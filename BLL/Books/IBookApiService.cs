using Models.Books;
using Models.Responses;

namespace BLL.Books
{
    public interface IBookApiService
    {
        Task<BLLResponse> CreateAsync(Book book);

        Task<BLLResponse> UpdateAsync(Book book);

        Task<BLLResponse> GetByLastUpdateAsync(DateTime lastUpdate, int page);
    }
}
