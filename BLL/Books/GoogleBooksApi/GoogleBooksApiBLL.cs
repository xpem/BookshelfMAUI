using ApiDAL;
using Models.Books.GoogleApi;
using Newtonsoft.Json;
using System.Collections;
using static Models.Books.GoogleApi.GoogleApiBook;

namespace BLL.Books.GoogleBooksApi
{
    public static class GoogleBooksApiBLL
    {

        /// <see cref="https://developers.google.com/books/docs/v1/reference#resource_volumes"/>
        public async static Task<(List<UIGoogleBook>, int)> GetBooks(string search, int startIndex)
        {
            try
            {
                Models.Responses.ApiResponse apiResponse = await GoogleBooksApiDAL.GetBooks(search, startIndex);
                if (apiResponse.Success && apiResponse.Content is not null)
                    return BuildListBooksResult(apiResponse.Content);
                else throw new Exception($"Erro não mapeado na resposta da api do google, content: {apiResponse.Content}");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async static Task<UIGoogleBook> GetBook(string googleId)
        {
            try
            {
                Models.Responses.ApiResponse apiResponse = await GoogleBooksApiDAL.GetBook(googleId);

                if (apiResponse.Success && apiResponse.Content is not null)
                {
                    Item? array = JsonConvert.DeserializeObject<Item>(apiResponse.Content);

                    if (array != null)
                        return BuildUIGoogleBook(array);
                }
                throw new Exception("Erro não mapeado na resposta da api do google");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static UIGoogleBook BuildUIGoogleBook(Item item)
        {
            UIGoogleBook uIGoogleBook;
            VolumeInfo volumeInfo;

            uIGoogleBook = new();

            volumeInfo = item.volumeInfo;

            uIGoogleBook.Id = item.id;

            uIGoogleBook.Title = volumeInfo.title;

            if (volumeInfo is not null)
            {
                uIGoogleBook.PageCount = volumeInfo.pageCount;


                if (volumeInfo.imageLinks?.smallThumbnail is not null)
                    uIGoogleBook.Thumbnail = volumeInfo.imageLinks.smallThumbnail;

                if (volumeInfo.publisher is not null)
                    uIGoogleBook.Publisher = volumeInfo.publisher;

                if (volumeInfo.publishedDate is not null)
                {
                    if (DateTime.TryParse((volumeInfo.publishedDate).ToString(), out DateTime publishedDate))
                        uIGoogleBook.PublishedDate = string.Format("{0:yyyy}", publishedDate);

                    else if (string.IsNullOrEmpty(volumeInfo.publishedDate))
                        uIGoogleBook.PublishedDate = volumeInfo.publishedDate;
                }

                if (volumeInfo.authors is not null)
                {
                    string strbdrarrays = "";
                    foreach (string itnAuthors in volumeInfo.authors)
                    {
                        if (string.IsNullOrEmpty(strbdrarrays.ToString()))
                            strbdrarrays = itnAuthors;
                        else
                            strbdrarrays += $"; {itnAuthors}";
                    }

                    uIGoogleBook.Authors = strbdrarrays;
                }
            }

            return uIGoogleBook;
        }

        private static (List<UIGoogleBook>, int) BuildListBooksResult(string json)
        {
            List<UIGoogleBook> list = [];
            int totalItems;

            try
            {
                GoogleApiBook? array = JsonConvert.DeserializeObject<GoogleApiBook>(json)
                    ?? throw new Exception("Erro não mapeado na resposta da api do google");

                List<Item>? items = array.items;

                totalItems = array.totalItems;


                if (totalItems > 0 && items != null)
                {
                    for (int i = 0; i < ((ICollection)array.items).Count; i++)
                    {
                        list.Add(BuildUIGoogleBook(items[i]));
                    }
                }
            }
            catch { throw; }

            return (list, totalItems);
        }
    }
}
