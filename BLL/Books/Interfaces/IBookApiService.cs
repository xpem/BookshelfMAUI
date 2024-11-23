using Models.DTOs;
using Models.Responses;

namespace Services.Books.Interfaces
{
    public interface IBookApiService
    {
        Task<BLLResponse> CreateAsync(Book book);

        Task<BLLResponse> UpdateAsync(Book book);

        Task<BLLResponse> GetByLastUpdateAsync(DateTime lastUpdate, int page);
    }
}
