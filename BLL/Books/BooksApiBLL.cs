using ApiDAL.Interfaces;
using BLL.Handlers;
using Models.Books;
using Models.Responses;
using System.Text.Json.Nodes;

namespace BLL.Books
{
    public class BooksApiBLL(IBookApiDAL bookApiDAL) : IBookApiBLL
    {
        public async Task<BLLResponse> AddBookAsync(Book book)
        {
            ApiResponse? resp = await bookApiDAL.AddBook(book);

            if (resp is not null)
            {
                if (resp.Success && resp.Content is not null)
                {
                    JsonNode? jResp = JsonNode.Parse(resp.Content);
                    if (jResp is not null)
                    {
                        int? addedBookId = null;

                        if (jResp != null)
                            addedBookId = jResp["id"]?.GetValue<int>();

                        return new BLLResponse() { Success = resp.Success, Content = addedBookId };
                    }
                    else return new BLLResponse() { Success = false, Content = resp.Content };
                }
                else
                {
                    if (resp.Content is not null)
                    {
                        JsonNode? jResp = JsonNode.Parse(resp.Content);
                        if (jResp is not null)
                        {
                            string? error = jResp["error"]?.GetValue<string>();
                            return new BLLResponse() { Success = false, Content = error };
                        }
                    }
                }
            }
            return new BLLResponse() { Success = false, Content = null };
        }

        public async Task<BLLResponse> UpdateBookAsync(Book book)
        {
            ApiResponse? resp = await bookApiDAL.UpdateBook(book);

            if (resp is not null && resp.Content is not null)
            {
                if (resp.Success)
                {
                    JsonNode? jResp = JsonNode.Parse(resp.Content);
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

        public async Task<BLLResponse> GetBooksByLastUpdateAsync(DateTime lastUpdate)
        {
            ApiResponse resp = await bookApiDAL.GetBooksByLastUpdate(lastUpdate);

            return ApiResponseHandler.Handler<List<Book>>(resp);
        }
    }
}
