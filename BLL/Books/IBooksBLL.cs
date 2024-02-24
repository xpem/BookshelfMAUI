using Models.Books;
using Models.Responses;

namespace BLL.Books
{
    public interface IBooksBLL
    {
        Task<Totals> GetBookshelfTotalsAsync(int uid);

        Task<Book?> GetBook(int localId);

        Task<BLLResponse> UpdateBook(Book book);

        Task<BLLResponse> AddBook(Book book);

        //bool VerifyBookbyTitle(string title);

        Book? GetBookbyTitleOrGoogleId(string title, string googleId);

        Task<(List<UIBookItem>, int total)> GetBooksByStatusAsync(int? page, int Situation, string? textoBusca);

        Task InactivateBookAsync(int LocalId);

        Task UpdateBookSituationAsync(int LocalId, Status status, int rate, string comment);
    }
}
