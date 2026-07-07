using Models.DTOs;
using Models.Responses;

namespace Services.Books.Interfaces
{
    public interface IBookApiService
    {
        Task<ServiceResponse> CreateAsync(Book book);

        Task<ServiceResponse> UpdateAsync(Book book);

        Task<ServiceResponse> GetByLastUpdateAsync(DateTime lastUpdate, int page);
    }
}
