using Models.Responses;
using System.Text.Json;

namespace BLL.Handlers
{
    public static class ApiResponseHandler
    {
        public static BLLResponse Handler<TModel>(ApiResponse apiResponse)
        {
            try
            {
                if (apiResponse is not null)
                {
                    if (!apiResponse.Success)
                    {
                        if (apiResponse.Error != null)
                            return new BLLResponse() { Success = false, TryRefreshToken = apiResponse.TryRefreshToken, Error = apiResponse.Error };
                        else if (apiResponse.Content != null)
                            return new BLLResponse() { Success = false, TryRefreshToken = apiResponse.TryRefreshToken, Content = apiResponse.Content };
                    }

                    if (apiResponse.Content is not null)
                        return new BLLResponse()
                        {
                            Success = true,
                            Content = string.IsNullOrEmpty(apiResponse.Content) ? null : JsonDeserialize<TModel>(apiResponse.Content)
                        };

                    throw new Exception("apiResponse.Content nulo");
                }

                throw new Exception("apiResponse nulo");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static TModel JsonDeserialize<TModel>(string content)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var item = JsonSerializer.Deserialize<TModel>(content, options);
                if (item is not null)
                    return item;
                else throw new Exception("item nulo");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

