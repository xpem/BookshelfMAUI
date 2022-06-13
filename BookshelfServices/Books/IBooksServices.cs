using BookshelfModels.Books;

namespace BookshelfServices.Books
{
    public interface IBooksServices
    {
        Task<Totals> GetBookshelfTotals();

        Task<Book?> GetBook(string bookKey);

        Task<string?> UpdateBook(Book book);

        Task<string?> AddBook(Book book);

        Task<bool> VerifyBookbyTitle(string title);

        Task<(List<UIBookItem>, int total)> GetBookSituationByStatus(int? page, int Situation, string? textoBusca);

        void InactivateBook(string bookKey);

        void UpdateBookSituation(string Key, Situation situation, int rate, string comment);
    }
}
