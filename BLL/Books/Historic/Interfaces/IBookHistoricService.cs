﻿using Models.Books.Historic;

namespace Services.Books.Historic.Interfaces
{
    public interface IBookHistoricService
    {
        Task<List<UIBookHistoric>> GetByBookIdAsync(int uid, int page, int bookId);
        Task<List<UIBookHistoric>> GetAsync(int uid, int page);
    }
}
