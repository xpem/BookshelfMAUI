using Models.Books;
using Models.DTOs;
using Models.Responses;

namespace Services.Books.Interfaces
{
    public interface IBookBLL
    {
        Task<BLLResponse> AddBookAsync(int uid, bool isOn, Book book);
        Task<bool> CheckIfExistsBookWithSameTitleAsync(int uid, string title, int? localId);
        Task<Book?> GetBookAsync(int uid, int localId);
        Task<Book?> GetBookbyTitleOrGoogleIdAsync(int uid, string title, string googleId);
        Task<List<UIBookItem>> GetBooksByStatusAsync(int uid, int page, int status, string? textoBusca = null);
        Task<Totals> GetBookshelfTotalsAsync(int uid);
        Task InactivateBookAsync(int uid, bool isOn, int localId);
        Task<BLLResponse> UpdateBookAsync(int uid, bool isOn, Book book);
        Task UpdateBookSituationAsync(int uid, bool isOn, int localId, Status status, int score, string comment);
    }
}
