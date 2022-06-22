using BookshelfModels.Books;
using BookshelfModels.Books.GoogleApi;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BookshelfModels.Books.GoogleApi.GoogleApiBook;

namespace BookshelfServices.Books.GoogleBooksApi
{
    public static class GoogleBooksApiService
    {

        /// <see cref="https://developers.google.com/books/docs/v1/reference#resource_volumes"/>
        public async static Task<List<UIGoogleBook>> GetBooks(string search, int index)
        {
            StringBuilder url = new();

            url.Append($"https://www.googleapis.com/books/v1/volumes?&startIndex={index}&q={search}&langRestrict=pt");

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

                string conteudo = await response.Content.ReadAsStringAsync();

                return BuildResult(conteudo.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static List<UIGoogleBook> BuildResult(string json)
        {
            List<UIGoogleBook> list = new();

            BookshelfModels.Books.GoogleApi.GoogleApiBook array = JsonConvert.DeserializeObject<GoogleApiBook>(json);

            if (array.totalItems > 0)
            {
                List<Item> items = array.items;

                if (items != null)
                {
                    int itemscount = ((ICollection)array.items).Count;

                    //
                    UIGoogleBook uIGoogleBook;
                    VolumeInfo volumeInfo;

                    for (int i = 0; i < itemscount; i++)
                    {
                        //variaveis utilizadas na criação da lista
                        uIGoogleBook = new();

                        volumeInfo = items[i].volumeInfo;

                        uIGoogleBook.Id = items[i].id;

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
                                    uIGoogleBook.PublishedDate = string.Format("{0:dd/MM/yyyy}", publishedDate);
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
                        list.Add(uIGoogleBook);
                    }
                }
            }
            return list;
        }
    }
}
