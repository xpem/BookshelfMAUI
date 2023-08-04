using Models.Books;

namespace LocalDbDAL.Books
{
    public interface IBookLocalDAL
    {
        Task AddBook(Book book, int? userId);
        Task AddOrUpdateBook(Book book, int? userId);
        Task<Book> GetBook(int userId, string bookKey);
        Task<Book?> GetBookByTitleOrGooglekey(int userId, string bookTitle, string? googleKey);
        Task<List<Book>> GetBooksByLastUpdate(int? userId, DateTime lastUpdate);
        Task<List<(Status, int)>> GetBookshelfTotals(int userId);
        Task<List<Book>> GetBookSituationByStatus(int Situation, int UserId, string? textoBusca);
        DateTime? GetLastUpdateBook(int? id, string? title);
        Task InactivateBook(int? bookId, int userId, DateTime lastUpdate);
        Task UpdateBook(Book book, int? userId);
        Task UpdateBookId(string localTempId, string? bookId, int? userId);
    }
}