using Models.Books;

namespace DbContextDAL
{
    public interface IBookDAL
    {
        int ExecuteAddBook(Book book);
        Task<int> ExecuteInactivateBookAsync(int localId, int userId);
        int ExecuteUpdateBook(Book book);
        Task<int> ExecuteUpdateBookStatusAsync(int localId, Status status, int score, string comment, int uid);
        List<Book> GetBookByAfterUpdatedAt(int uid, DateTime lastUpdate);
        Task<Book?> GetBookByLocalIdAsync(int uid, int localId);
        Task<Book?> GetBookByTitleAsync(int uid, string title);
        Book? GetBookByTitleOrGoogleId(int uid, string title, string googleId);
        Task<List<Book>> GetBooks(int uid);
        List<Book> GetBooksByStatus(int uid, Status status);
        DateTime? GetBookUpdatedAtById(int id);
        Totals GetTotalBooksGroupedByStatus(int uid);
    }
}