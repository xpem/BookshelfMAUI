using Models.Books;
using Models.DTOs;

namespace Repos.Interfaces
{
    public interface IBookRepo
    {
        Task<int> CreateAsync(Book book);

        Task<int> ExecuteInactivateBookAsync(int localId, int userId);

        Task<int> UpdateAsync(Book book);

        Task<int> ExecuteUpdateBookStatusAsync(int localId, Status status, int score, string comment, int uid);

        //Task<List<Book>> GetBookByAfterUpdatedAtAsync(int uid, DateTime lastUpdate);

        Task<Book?> GetBookByLocalIdAsync(int uid, int localId);

        //Task<Book?> GetBookByLocalTempIdAsync(int uid, string localTempId);

        Task<Book?> GetByTitleAsync(int uid, string title);

        Task<Book?> GetByIdAsync(int id);

        Task<List<TotalBooksGroupedByStatus>> GetTotalBooksGroupedByStatusAsync(int uid);

        Task<List<Book>> GetAsync(int uid, int page, string? searchTitle);

        Task<List<Book>> GetByStatusAsync(int uid, Status status, int page, string? searchTitle);

        Task<Book?> GetByTitleOrGoogleIdAsync(int uid, string title, string? googleId = null);

        Task<bool> CheckIfExistsWithSameTitleAsync(int uid, string title, int? localId);
    }
}