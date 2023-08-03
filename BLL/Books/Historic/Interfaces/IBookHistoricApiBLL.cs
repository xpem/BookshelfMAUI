using Models.Responses;

namespace BLL.Books.Historic.Interfaces
{
    public interface IBookHistoricApiBLL
    {
        Task<BLLResponse> GetBookHistoricByLastCreatedAt(DateTime lastCreatedAt);
    }
}
