﻿using ApiDAL.Interfaces;
using Models.DTOs;
using Models.Responses;
using Services.Books.Interfaces;
using Services.Handlers;
using System.Text.Json.Nodes;

namespace Services.Books
{
    public class BooksApiService(IBookApiRepo bookApiRepo) : IBookApiService
    {
        public async Task<BLLResponse> CreateAsync(Book book)
        {
            ApiResponse? resp = await bookApiRepo.CreateAsync(book);

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

        public async Task<BLLResponse> UpdateAsync(Book book)
        {
            ApiResponse? resp = await bookApiRepo.UpdateAsync(book);

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

        public async Task<BLLResponse> GetByLastUpdateAsync(DateTime lastUpdate, int page)
        {
            ApiResponse resp = await bookApiRepo.GetByLastUpdateAsync(lastUpdate, page);

            return ApiResponseHandler.Handler<List<Book>>(resp);
        }
    }
}
