﻿using BookshelfModels.Books;
using Microsoft.Data.Sqlite;

namespace BookshelfRepos.Books
{
    public static class BooksRepos
    {
        public static async Task<List<Book>> GetBooksByLastUpdate(string? userId, DateTime lastUpdate)
        {
            try
            {
                SQLiteDB.OpenIfClosed();

                List<SqliteParameter> parameters = new()
                {
                    new SqliteParameter("@UserId", userId),
                    new SqliteParameter("@LastUpdate", lastUpdate.ToString("yyyy-MM-dd hh:mm:ss"))
                };

                SqliteDataReader response = await SQLiteDB.RunSqliteCommand("select b.Title, b.Authors, b.Year, b.Volume, b.Pages, b.Genre, b.LastUpdate, b.SubTitle, b.Isbn, b.Situation, br.Rate, " +
                    " br.comment, b.Key, b.Inactive from BOOK b inner join BOOKRATING br on br.BookKey = b.key where b.UserId = @UserId and" +
                    " b.lastUpdate > @LastUpdate", parameters);

                List<Book> lista = new();

                while (response.Read())
                {
                    lista.Add(new Book()
                    {
                        Title = response.GetWithNullableString(0),
                        Authors = response.GetWithNullableString(1),
                        Year = response.GetInt32(2),
                        Volume = response.GetWithNullableString(3),
                        Pages = response.GetInt32(4),
                        Genre = response.GetWithNullableString(5),
                        LastUpdate = Convert.ToDateTime(response.GetWithNullableString(6)),
                        SubTitle = response.GetWithNullableString(7),
                        Isbn = response.GetWithNullableString(8),
                        Situation = (Situation)response.GetInt32(9),
                        Rating = new Rating() { Rate = response.GetWithNullableInt(10), Comment = response.GetWithNullableString(11) },
                        BookKey = response.GetWithNullableString(12),
                        Inactive = response.GetWithNullableBool(13)
                    });
                }

                SQLiteDB.CloseIfOpen();

                return lista;
            }
            catch (Exception ex) { throw ex; }
        }

        public static void UpdateBookKey(Guid localBookKey, string bookKey, string? userId)
        {
            SQLiteDB.OpenIfClosed();

            List<SqliteParameter> sqliteParameters = new()
            {
                new SqliteParameter("@Key", bookKey), new SqliteParameter("@localKey", localBookKey.ToString()), new SqliteParameter("@userId", userId),
            };

            _ = SQLiteDB.RunSqliteCommand("update BOOK set Key = @Key where KEY = @localKey and UserId = @userId", sqliteParameters);

            _ = SQLiteDB.RunSqliteCommand("update BOOKRATING set BookKey = @Key where BookKey = @localKey", sqliteParameters);

            SQLiteDB.CloseIfOpen();
        }

        public static void AddBook(Book book, string? userId)
        {
            SQLiteDB.OpenIfClosed();

            List<SqliteParameter> sqliteParametersList = new()
            {
                new SqliteParameter("@Key", book.BookKey),
                new SqliteParameter("@UserId", userId),
                new SqliteParameter("@Title", book.Title),
                new SqliteParameter("@SubTitle", book.SubTitle),
                new SqliteParameter("@Authors", book.Authors),
                new SqliteParameter("@Year", book.Year),
                new SqliteParameter("@Volume", book.Volume),
                new SqliteParameter("@Pages", book.Pages),
                new SqliteParameter("@Genre", book.Genre),
                new SqliteParameter("@LastUpdate", book.LastUpdate),
                new SqliteParameter("@Isbn", book.Isbn),
                new SqliteParameter("@Situation", book.Situation)
            };

            _ = SQLiteDB.RunSqliteCommand(
                "insert into BOOK(Key, UserId, Title, SubTitle, Authors, Year, Volume, Pages, Genre, LastUpdate, Isbn, Situation) " +
                "values (@Key, @UserId, @Title, @SubTitle, @Authors, @Year, @Volume, @Pages, @Genre, @LastUpdate,@Isbn,@Situation)",
                sqliteParametersList);

            sqliteParametersList = new() {
                new SqliteParameter("@BookKey", book.BookKey),
                new SqliteParameter("@Rate", book.Rating?.Rate),
                new SqliteParameter("@Comment", book.Rating?.Comment)
            };

            _ = SQLiteDB.RunSqliteCommand("insert into BOOKRATING(BookKey,Rate,Comment)" +
                " values (@BookKey, @Rate, @Comment)", sqliteParametersList
               );

            SQLiteDB.CloseIfOpen();
        }

        /// <summary>
        /// update book local
        /// </summary>
        /// <param name="book"></param>
        public static void UpdateBook(Book book, string? userId)
        {
            SQLiteDB.OpenIfClosed();

            List<SqliteParameter> sqliteParameters = new()
            {
                new SqliteParameter("@Key", book.BookKey),
                new SqliteParameter("@UserId", userId),
                new SqliteParameter("@Title", book.Title),
                new SqliteParameter("@SubTitle", book.SubTitle),
                new SqliteParameter("@Authors", book.Authors),
                new SqliteParameter("@Year", book.Year),
                new SqliteParameter("@Volume", book.Volume),
                new SqliteParameter("@Pages", book.Pages),
                new SqliteParameter("@Genre", book.Genre),
                new SqliteParameter("@LastUpdate", book.LastUpdate),
                new SqliteParameter("@Isbn", book.Isbn),
                new SqliteParameter("@Inactive", (book.Inactive ? "1" : "0")),
                new SqliteParameter("@Situation", book.Situation)
            };

            _ = SQLiteDB.RunSqliteCommand("update BOOK set Title = @Title, SubTitle = @SubTitle, Authors = @Authors, Year = @Year, Volume = @Volume, Pages = @Pages" +
                         ", Genre = @Genre, LastUpdate = @LastUpdate,Isbn = @Isbn,Inactive = @Inactive,Situation = @Situation where KEY = @Key and UserId = @UserId",
                        sqliteParameters);

            sqliteParameters = new()
            {
                new SqliteParameter("@Key", book.BookKey),
                new SqliteParameter("@Rate", book.Rating?.Rate),
                new SqliteParameter("@Comment", book.Rating?.Comment)
            };

            _ = SQLiteDB.RunSqliteCommand("update BOOKRATING set Rate = @Rate,Comment = @Comment where BookKey = @Key", sqliteParameters);

            SQLiteDB.CloseIfOpen();
        }

        public static DateTime? GetLastUpdateBook(string? KEY, string? Title)
        {
            SQLiteDB.OpenIfClosed();
            string command = "select LastUpdate from BOOK where";

            List<SqliteParameter> parameters = new();

            if (!string.IsNullOrEmpty(KEY))
            {
                command += " KEY = @Key";
            }
            else
            {
                command += " title = @Title";
            }

            if (!string.IsNullOrEmpty(KEY))
            {
                parameters.Add(new SqliteParameter("@Key", KEY));
            }
            else
            {
                parameters.Add(new SqliteParameter("@Title", Title));
            }

            SqliteDataReader response = Task.Run(async () => await SQLiteDB.RunSqliteCommand(command, parameters)).Result;

            _ = response.Read();

            string? lastUpdate = "";

            if (response.HasRows)
            {
                lastUpdate = response.GetWithNullableString(0);
            }

            SQLiteDB.CloseIfOpen();

            if (string.IsNullOrEmpty(lastUpdate)) return null;
            else return Convert.ToDateTime(lastUpdate);
        }

        /// <summary>
        /// Update or create local book with the newest version of the book in the server
        /// </summary>
        /// <param name="book"></param>
        public static void AddOrUpdateBook(Book book, string? userId)
        {
            DateTime? lastUpdate = GetLastUpdateBook(book?.BookKey, book?.Title);

            if (lastUpdate == null && book is not null && !book.Inactive)
                AddBook(book, userId);
            else if (book is not null && book.LastUpdate > lastUpdate)
                UpdateBook(book, userId);
        }

        public async static Task<List<(Situation, int)>> GetBookshelfTotals(string userId)
        {
            SQLiteDB.OpenIfClosed();

            List<(Situation, int)> booksTotalSituations = new();

            SqliteDataReader response = await SQLiteDB.RunSqliteCommand("select situation,count(Situation) from BOOK where UserId = @UserId and (Inactive is null or Inactive = '0')  group by situation",
                new List<SqliteParameter>() { new SqliteParameter("@UserId", userId) });

            while (response.Read())
            {
                booksTotalSituations.Add(((Situation)response.GetInt32(0), response.GetInt32(1)));
            }

            SQLiteDB.CloseIfOpen();

            return booksTotalSituations;
        }
    }
}
