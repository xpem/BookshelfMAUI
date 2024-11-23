using Models.DTOs;
using Models.Responses;

namespace ApiDAL.Interfaces
{
    public interface IBookApiRepo
    {
        Task<ApiResponse> CreateAsync(Book book);

        Task<ApiResponse> UpdateAsync(Book book);

        Task<ApiResponse> GetByLastUpdateAsync(DateTime lastUpdate, int page);
    }
}
