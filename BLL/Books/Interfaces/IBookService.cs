using Models.Books;
using Models.DTOs;
using Models.Responses;

namespace Services.Books.Interfaces
{
    public interface IBookService
    {
        Task<BLLResponse> AddAsync(int uid, bool isOn, Book book);
        Task<bool> CheckIfExistsBookWithSameTitleAsync(int uid, string title, int? localId);
        Task<Book?> GetAsync(int uid, int localId);
        Task<Book?> GetbyTitleOrGoogleIdAsync(int uid, string title, string googleId);
        Task<List<UIBookItem>> GetByStatusAsync(int uid, int page, int status, string? textoBusca = null);
        Task<Totals> GetBookshelfTotalsAsync(int uid);
        Task InactivateBookAsync(int uid, bool isOn, int localId);
        Task<BLLResponse> UpdateAsync(int uid, bool isOn, Book book);
        Task UpdateBookSituationAsync(int uid, bool isOn, int localId, Status status, int score, string comment);
    }
}
