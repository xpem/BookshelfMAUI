using ApiDAL;
using BLL.Handlers;
using BookshelfModels.Books;
using Models.Responses;
using System.Text.Json.Nodes;

namespace BLL.Books.Api
{
    public static class BooksApiBLL
    {
        public static async Task<BLLResponse> AddBook(Book book)
        {
            var resp = await BookApiDAL.AddBook(book);

            if (resp is not null && resp.Success && resp.Content is not null)
            {
                var jResp = JsonNode.Parse(resp.Content);
                if (jResp is not null)
                {
                    int? addedBookId = null;
                    if (jResp != null)
                        addedBookId = jResp["Id"]?.GetValue<int>();

                    return new BLLResponse() { Success = resp.Success, Content = addedBookId };
                }
                else return new BLLResponse() { Success = false, Content = resp.Content };
            }

            return new BLLResponse() { Success = false, Content = null };
        }

        public static async Task<BLLResponse> AltBook(Book book)
        {
            var resp = await BookApiDAL.AltBook(book);

            if (resp is not null && resp.Content is not null)
            {
                if (resp.Success)
                {
                    var jResp = JsonNode.Parse(resp.Content);
                    if (jResp is not null)
                    {
                        int bookId = jResp["Id"]?.GetValue<int>() ?? 0;
                        return new BLLResponse() { Success = resp.Success, Content = bookId };
                    }

                    return new BLLResponse() { Success = resp.Success, Content = string.Empty };
                }
                else return new BLLResponse() { Success = false, Content = resp.Content };
            }

            return new BLLResponse() { Success = false, Content = null };
        }

        public static async Task<BLLResponse> GetBooksByLastUpdate(DateTime lastUpdate)
        {
            var resp = await BookApiDAL.GetBooksByLastUpdate(lastUpdate);

            return ApiResponseHandler.Handler<List<Book>>(resp);          
        }
    }
}
