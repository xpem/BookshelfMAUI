using Models.Responses;

namespace BLL.Books.Historic
{
    public interface IBookHistoricApiBLL
    {
        Task<BLLResponse> GetBookHistoricByLastCreatedAt(DateTime lastCreatedAt);
    }
}
