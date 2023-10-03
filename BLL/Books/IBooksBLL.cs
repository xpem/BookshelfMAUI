using Models.Books;
using Models.Responses;

namespace BLL.Books
{
    public interface IBooksBLL
    {
        Totals GetBookshelfTotals();

        Task<Book?> GetBook(int localId);

        Task<BLLResponse> UpdateBook(Book book);

        Task<BLLResponse> AddBook(Book book);

        //bool VerifyBookbyTitle(string title);

        Task<Book?> GetBookbyTitleOrGoogleIdAsync(string title, string googleId);

        Task<(List<UIBookItem>, int total)> GetBookSituationByStatus(int? page, int Situation, string? textoBusca);

        Task InactivateBook(int LocalId);

        Task UpdateBookSituation(int LocalId, Status status, int rate, string comment);
    }
}
