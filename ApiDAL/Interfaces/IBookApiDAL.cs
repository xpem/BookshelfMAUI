using Models.Books;
using Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiDAL.Interfaces
{
    public interface IBookApiDAL
    {
        Task<ApiResponse> AddBook(Book book);

        Task<ApiResponse> AltBook(Book book);

        Task<ApiResponse> GetBooksByLastUpdate(DateTime lastUpdate);
    }
}
