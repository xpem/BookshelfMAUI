using Models.Books;
using Models.Responses;

namespace BLL.Books
{
    public interface IBooksBLL
    {
        Task<Totals> GetBookshelfTotalsAsync(int uid);

        Task<Book?> GetBookAsync(int uid,int localId);

        Task<BLLResponse> UpdateBookAsync(int uid, Book book);

        Task<BLLResponse> AddBookAsync(int uid, Book book);

        //bool VerifyBookbyTitle(string title);

        Book? GetBookbyTitleOrGoogleId(int uid, string title, string googleId);

        Task<List<UIBookItem>> GetBooksByStatusAsync(int uid, int page, int Situation, string? textoBusca);

        Task InactivateBookAsync(int uid, int LocalId);

        Task UpdateBookSituationAsync(int uid, int LocalId, Status status, int rate, string comment);
    }
}
