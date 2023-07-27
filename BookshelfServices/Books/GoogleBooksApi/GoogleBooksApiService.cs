using BookshelfModels.Books.GoogleApi;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using static BookshelfModels.Books.GoogleApi.GoogleApiBook;

namespace BLL.Books.GoogleBooksApi
{
    public static class GoogleBooksApiService
    {

        /// <see cref="https://developers.google.com/books/docs/v1/reference#resource_volumes"/>
        public async static Task<(List<UIGoogleBook>, int)> GetBooks(string search, int startIndex)
        {
            StringBuilder url = new();

            url.Append($"https://www.googleapis.com/books/v1/volumes?&startIndex={startIndex}&q={search}&langRestrict=pt&printType=books");

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
                HttpResponseMessage response = await new HttpClient().GetAsync(url.ToString());

                return BuildResult(await response.Content.ReadAsStringAsync());
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
                HttpResponseMessage response = await new HttpClient().GetAsync($"https://www.googleapis.com/books/v1/volumes/{googleId}");
                Item? array = JsonConvert.DeserializeObject<Item>(await response.Content.ReadAsStringAsync());

                var item = BuildUIGoogleBook(array);
                return item;
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
                    {
                        uIGoogleBook.PublishedDate = string.Format("{0:yyyy}", publishedDate);
                    }
                    else if (string.IsNullOrEmpty(volumeInfo.publishedDate))
                    {
                        uIGoogleBook.PublishedDate = volumeInfo.publishedDate;
                    }
                }

                if (volumeInfo.authors is not null)
                {
                    string strbdrarrays = "";
                    foreach (string itnAuthors in volumeInfo.authors)
                    {
                        if (string.IsNullOrEmpty(strbdrarrays.ToString()))
                        {
                            strbdrarrays = itnAuthors;
                        }
                        else
                        {
                            strbdrarrays += $"; {itnAuthors}";
                        }
                    }

                    uIGoogleBook.Authors = strbdrarrays;
                }
            }

            return uIGoogleBook;
        }

        private static (List<UIGoogleBook>, int) BuildResult(string json)
        {
            List<UIGoogleBook> list = new();
            int totalItems;

            try
            {
                BookshelfModels.Books.GoogleApi.GoogleApiBook? array = JsonConvert.DeserializeObject<GoogleApiBook>(json);
                List<Item> items = array.items;
                totalItems = array.totalItems;

                if (totalItems > 0)
                {
                    if (items != null)
                    {
                        for (int i = 0; i < ((ICollection)array.items).Count; i++)
                        {
                            list.Add(BuildUIGoogleBook(items[i]));
                        }
                    }
                }
            }
            catch (Exception ex) { throw ex; }

            return (list, totalItems);
        }
    }
}
