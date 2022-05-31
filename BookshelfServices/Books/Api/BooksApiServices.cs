using BookshelfModels.Books;
using BookshelfServices.User;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace BookshelfServices.Books.Api
{
    public class BooksApiServices : IBooksApiServices
    {
        static HttpClient httpClient = new();
        readonly IUserServices userServices;

        public BooksApiServices(IUserServices _userServices)
        {
            userServices = _userServices;
        }

        /// <summary>
        /// Add a book in fb bd
        /// </summary>
        /// <param name="book"></param>
        /// <returns>Key of book added</returns>
        public async Task<(bool, string)> AddBook(Book book, BookshelfModels.User.User? user)
        {
            try
            {
                int forContinue = 0;

                while (forContinue < 2)
                {
                    var json = JsonConvert.SerializeObject(book);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");

                    //to do - make a base to this
                    httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Add("Authorization", user?.Token);
                    HttpResponseMessage response = await httpClient.PostAsync(ApiKeys.ApiUri + "/InsertBook", data);
                    var result = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode)
                    {
                        JObject obj = JObject.Parse(result);
                        return (true, (string)obj["BookKey"]);
                    }
                    else
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            user = await userServices.RefreshUserToken(user);
                            //retry get
                            forContinue++;
                        }
                        else
                        {
                            JObject obj = JObject.Parse(result);
                            return (false, (string)obj["message"]);
                        }
                    }
                }

                return (false, "O servidor está indisponível");
            }
            catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Modify a book in fb bd
        /// </summary>
        /// <param name="book"></param>
        public async Task<(bool, string)> UpdateBook(Book book, BookshelfModels.User.User? user)
        {
            try
            {
                int forContinue = 0;

                while (forContinue < 2)
                {
                    var json = JsonConvert.SerializeObject(book);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");

                    httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Add("Authorization", user?.Token);
                    HttpResponseMessage response = await httpClient.PutAsync(ApiKeys.ApiUri + "/UpdateBook/" + book.BookKey, data);
                    var result = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode)
                    {
                        return (true, string.Empty);
                    }
                    else
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            user = await userServices.RefreshUserToken(user);
                            //retry get
                            forContinue++;
                        }
                        else
                        {
                            JObject obj = JObject.Parse(result);
                            return (false, (string)obj["message"]);
                        }
                    }
                }

                return (false, "O servidor está indisponível");
            }
            catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// recover list of books by last update datetime
        /// </summary>
        /// <param name="vUserKey"></param>
        /// <param name="vLastUpdate"></param>
        /// <returns></returns>
        public async Task<List<Book>?> GetBooksByLastUpdate(BookshelfModels.User.User? user)
        {
            try
            {
                int forContinue = 0;

                while (forContinue < 2)
                {
                    httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Add("Authorization", user?.Token);
                    httpClient.DefaultRequestHeaders.Add("lastUpdate", user?.LastUpdate.ToString("yyyy-MM-ddThh:mm:ss"));
                    HttpResponseMessage response = await httpClient.GetAsync(ApiKeys.ApiUri + "/GetBooksByLastUpdate");

                    if (response.IsSuccessStatusCode)
                    {
                        string result = response.Content.ReadAsStringAsync().Result;
                        return (JsonConvert.DeserializeObject<List<Book>>(result));
                    }
                    else
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            user = await userServices.RefreshUserToken(user);
                            //retry get
                            forContinue++;
                        }
                        else return null;
                    }
                }
                return null;
            }
            catch (Exception ex) { throw ex; }
        }



    }
}
