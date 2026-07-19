using Models.Responses;
using System.Net;
using System.Text;

namespace ApiRepo
{
    public class GoogleBooksApiRepo
    {
        private const string BASEURL = "https://www.googleapis.com/books/v1/volumes";

        /// <see cref="https://developers.google.com/books/docs/v1/reference#resource_volumes"/>
        public async static Task<ApiResp> GetBooks(string search, int startIndex)
        {
            StringBuilder url = new();

            url.Append($"{BASEURL}?&startIndex={startIndex}&q=intitle:{search}&langRestrict=pt&printType=books&orderBy=relevance");

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

                return new ApiResp()
                {
                    Success = httpResponse.IsSuccessStatusCode,
                    ErrorCode = httpResponse.StatusCode == HttpStatusCode.Unauthorized ? ErrorCodeTypes.Unauthorized : ErrorCodeTypes.Unknown,
                    Content = await httpResponse.Content.ReadAsStringAsync()
                };
            }
            catch { throw; }
        }
    }
}
