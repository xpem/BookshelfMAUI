using Models.Books;
using Models.Responses;

namespace BLL.Books
{
    public interface IBooksBLL
    {
        Totals GetBookshelfTotals();

        Task<Book?> GetBook(int bookId);

        Task<BLLResponse> UpdateBook(Book book);

        Task<BLLResponse> AddBook(Book book);

        //bool VerifyBookbyTitle(string title);

        Book? GetBookbyTitleOrGoogleId(string title, string googleId);

        Task<(List<UIBookItem>, int total)> GetBookSituationByStatus(int? page, int Situation, string? textoBusca);

        Task InactivateBook(int bookId);

        Task UpdateBookSituation(int bookId, Status status, int rate, string comment);
    }
}
