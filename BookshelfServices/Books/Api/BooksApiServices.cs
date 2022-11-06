using BookshelfModels.Books;
using BookshelfServices.User;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;

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
        public async Task<(bool, string?)> AddBook(Book book, BookshelfModels.User.User? user)
        {
            try
            {
                int forContinue = 0;

                while (forContinue < 2)
                {
                    var json = JsonSerializer.Serialize(book);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");

                    //to do - make a base to this
                    httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Add("authorization", "bearer " + user?.Token);
                    HttpResponseMessage response = await httpClient.PostAsync(ApiKeys.ApiUri + "/Book", data);
                    var result = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var obj = JsonNode.Parse(result);

                        if (obj != null)
                            return (true, (obj["Id"]?.GetValue<int>()).ToString());
                        else throw new Exception(result);
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
                            var obj = JsonNode.Parse(result);
                            if (obj != null)
                                return (false, obj["message"]?.ToJsonString());
                            else throw new Exception(result);
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
        public async Task<(bool, string?)> UpdateBook(Book book, BookshelfModels.User.User? user)
        {
            try
            {
                int forContinue = 0;

                while (forContinue < 2)
                {
                    var json = JsonSerializer.Serialize(book);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");

                    httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Add("Authorization","bearer "+ user?.Token);
                    HttpResponseMessage response = await httpClient.PutAsync(ApiKeys.ApiUri + "/Book/" + book.Id, data);
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
                            var obj = JsonNode.Parse(result);
                            if (obj != null)
                                return (false, obj["message"]?.ToJsonString());
                            else throw new Exception(result);
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
        public async Task<List<Book>?> GetBooksByLastUpdate(BookshelfModels.User.User user)
        {
            try
            {
                int forContinue = 0;

                while (forContinue < 2)
                {
                    httpClient = new();
                    httpClient.DefaultRequestHeaders.Add("Authorization", "bearer " + user.Token);
                    HttpResponseMessage response = await httpClient.GetAsync(ApiKeys.ApiUri + "/book/byupdatedat/" + user.LastUpdate.ToString("yyyy-MM-ddThh:mm:ss.fff"));

                    if (response.IsSuccessStatusCode)
                        return await response.Content.ReadFromJsonAsync<List<Book>>();
                    else
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            await userServices.RefreshUserToken(user);
                        }
                        else return null;
                    }
                    forContinue++;
                }
                return null;
            }
            catch (Exception ex) { throw ex; }
        }



    }
}
