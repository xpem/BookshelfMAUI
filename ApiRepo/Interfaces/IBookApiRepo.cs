using Models.DTOs;
using Models.Responses;

namespace ApiRepo.Interfaces
{
    public interface IBookApiRepo
    {
        Task<ApiResp> CreateAsync(Book book);

        Task<ApiResp> UpdateAsync(Book book);

        Task<ApiResp> GetByLastUpdateAsync(DateTime lastUpdate, int page);
    }
}
