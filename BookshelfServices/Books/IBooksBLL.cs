﻿using BookshelfModels.Books;

namespace BookshelfServices.Books
{
    public interface IBooksBLL
    {
        Task<Totals> GetBookshelfTotals();

        Task<Book?> GetBook(string bookKey);

        Task<bool> AltBook(Book book);

        Task<bool> AddBook(Book book);

        Task<bool> VerifyBookbyTitle(string title);

        Task<Book?> GetBookbyTitleAndGoogleId(string title, string googleId);

        Task<(List<UIBookItem>, int total)> GetBookSituationByStatus(int? page, int Situation, string? textoBusca);

        Task InactivateBook(string bookKey);

        Task UpdateBookSituation(string Key, Status status, int rate, string comment);
    }
}