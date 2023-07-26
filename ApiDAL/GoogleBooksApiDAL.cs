using BookshelfModels.Books.GoogleApi;
using Models.Responses;
using System.Net;
using System.Text;

namespace ApiDAL
{
    public class GoogleBooksApiDAL
    {
        private const string BASEURL = "https://www.googleapis.com/books/v1/volumes";

        /// <see cref="https://developers.google.com/books/docs/v1/reference#resource_volumes"/>
        public async static Task<ApiResponse> GetBooks(string search, int startIndex)
        {
            StringBuilder url = new();

            url.Append($"{BASEURL}?&startIndex={startIndex}&q={search}&langRestrict=pt&printType=books");

            //for a custom search
            //switch (key)
            //{
            //    case 0:
            //        url.Append("+intitle:" + buscakey);
            //        break;
            //    case 1:
            //        url.Append("+inauthor:" + buscakey);
            //        break;
            //    case 2:
            //        url.Append("+subject:" + buscakey);                  
            //        break;
            //    default:
            //        break;
            //}
            //url.Append("&langRestrict=pt");

            try
            {
                HttpResponseMessage httpResponse = await new HttpClient().GetAsync(url.ToString());

                return new ApiResponse()
                {
                    Success = httpResponse.IsSuccessStatusCode,
                    Error = httpResponse.StatusCode == HttpStatusCode.Unauthorized ? ErrorTypes.Unauthorized : null,
                    Content = await httpResponse.Content.ReadAsStringAsync()
                };
            }
            catch { throw; }
        }

        public async static Task<ApiResponse> GetBook(string googleId)
        {
            HttpResponseMessage httpResponse = await new HttpClient().GetAsync($"{BASEURL}/{googleId}");

            return new ApiResponse()
            {
                Success = httpResponse.IsSuccessStatusCode,
                Error = httpResponse.StatusCode == HttpStatusCode.Unauthorized ? ErrorTypes.Unauthorized : null,
                Content = await httpResponse.Content.ReadAsStringAsync()
            };
        }
    }
}
