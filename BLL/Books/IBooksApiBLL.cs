using ApiDAL;
using BLL.Handlers;
using Models.Books;
using Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace BLL.Books
{
    public interface IBooksApiBLL
    {
         Task<BLLResponse> AddBook(Book book);

        Task<BLLResponse> AltBook(Book book);

        Task<BLLResponse> GetBooksByLastUpdate(DateTime lastUpdate);
    }
}
