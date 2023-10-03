using Models.Books;

namespace DbContextDAL
{
    public interface IBookDAL
    {
        Task<int> ExecuteAddBookAsync(Book book);
        Task<int> ExecuteInactivateBookAsync(int localId, int userId);
        Task<int> ExecuteUpdateBookAsync(Book book);
        Task<int> ExecuteUpdateBookStatusAsync(int localId, Status status, int score, string comment, int uid);
        Task<Book?> GetBookByLocalIdAsync(int uid, int localId);
        Task<Book?> GetBookByTitleAsync(int uid, string title);
        Task<Book?> GetBookbyTitleOrGoogleIdAsync(int uid, string title, string googleId);
        Task<List<Book>> GetBooks(int uid);
        Task<List<Book>> GetBooksByStatusAsync(int uid, Status status);
        Totals GetTotalBooksGroupedByStatus(int uid);
    }
}