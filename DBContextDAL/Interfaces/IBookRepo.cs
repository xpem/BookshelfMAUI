﻿using Models.Books;
using Models.DTOs;

namespace Repositories.Interfaces
{
    public interface IBookRepo
    {
        Task<int> CreateAsyn(Book book);

        Task<int> ExecuteInactivateBookAsync(int localId, int userId);

        Task<int> UpdateAsync(Book book);

        Task<int> ExecuteUpdateBookStatusAsync(int localId, Status status, int score, string comment, int uid);

        //Task<List<Book>> GetBookByAfterUpdatedAtAsync(int uid, DateTime lastUpdate);

        Task<Book?> GetBookByLocalIdAsync(int uid, int localId);

        Task<Book?> GetBookByLocalTempIdAsync(int uid, string localTempId);

        Task<Book?> GetBookByTitleAsync(int uid, string title);

        Task<DateTime?> GetUpdatedAtByIdAsync(int id);

        Task<List<TotalBooksGroupedByStatus>> GetTotalBooksGroupedByStatusAsync(int uid);

        Task<List<Book>> GetBooksAsync(int uid, int page);

        Task<List<Book>> GetBooksByStatusAsync(int uid, Status status, int page);

        Task<Book?> GetBookByTitleOrGoogleIdAsync(int uid, string title, string? googleId = null);

        Task<bool> CheckIfExistsBookWithSameTitleAsync(int uid, string title, int? localId);
    }
}