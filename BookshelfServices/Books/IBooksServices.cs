using BookshelfModels.Books;

namespace BLL.Books
{
    public interface IBooksServices
    {
        Task<Totals> GetBookshelfTotals();

        Task<Book?> GetBook(string bookKey);

        Task<string?> UpdateBook(Book book);

        Task<string?> AddBook(Book book);

        Task<bool> VerifyBookbyTitle(string title);

        Task<Book?> GetBookbyTitleAndGoogleId(string title, string googleId);

        Task<(List<UIBookItem>, int total)> GetBookSituationByStatus(int? page, int Situation, string? textoBusca);

        void InactivateBook(string bookKey);

        void UpdateBookSituation(string Key, Status status, int rate, string comment);
    }
}
