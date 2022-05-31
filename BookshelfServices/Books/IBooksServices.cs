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
    }
}
