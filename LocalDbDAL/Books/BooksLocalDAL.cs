using BookshelfModels.Books;
using Microsoft.Data.Sqlite;
using System.Text;

namespace BookshelfRepos.Books
{
    public static class BooksLocalDAL
    {
        public static async Task<List<Book>> GetBooksByLastUpdate(int? userId, DateTime lastUpdate)
        {
            try
            {
                SqliteFunctions.OpenIfClosed();

                string _lastUpdate = lastUpdate.ToString("yyyy-MM-dd hh:mm:ss.fff");

                List<SqliteParameter> parameters = new()
                {
                    new SqliteParameter("@UserId", userId),
                    new SqliteParameter("@LastUpdate", _lastUpdate)
                };

                SqliteDataReader response = await SqliteFunctions.RunSqliteCommand("select ID, LOCAL_TEMP_ID, TITLE, SUBTITLE, AUTHORS, YEAR, VOLUME, PAGES, ISBN, GENRE, UPDATED_AT, INACTIVE, STATUS, COVER, GOOGLE_ID, SCORE, COMMENT, CREATED_AT from BOOK where UID = @UserId and" +
                    " UPDATED_AT > @LastUpdate", parameters);

                List<Book> lista = new();

                while (response.Read())
                {
                    lista.Add(new Book()
                    {
                        Id = response.GetInt32(0),
                        LocalTempId = response.GetWithNullableString(1),
                        Title = response.GetWithNullableString(2),
                        SubTitle = response.GetWithNullableString(3),
                        Authors = response.GetWithNullableString(4),
                        Year = response.GetWithNullableInt(5),
                        Volume = response.GetWithNullableInt(6),
                        Pages = response.GetInt32(7),
                        Isbn = response.GetWithNullableString(8),
                        Genre = response.GetWithNullableString(9),
                        UpdatedAt = Convert.ToDateTime(response.GetWithNullableString(10)),
                        Inactive = response.GetWithNullableInt(11),
                        Status = (Status)response.GetInt32(12),
                        Cover = response.GetWithNullableString(13),
                        GoogleId = response.GetWithNullableString(14),
                        Score = response.GetWithNullableInt(15),
                        Comment = response.GetWithNullableString(16),
                        CreatedAt = Convert.ToDateTime(response.GetWithNullableString(17)),
                    });
                }

                SqliteFunctions.CloseIfOpen();

                return lista;
            }
            catch (Exception ex) { throw ex; }
        }

        public static async Task UpdateBookId(string localTempId, string? bookId, int? userId)
        {
            SqliteFunctions.OpenIfClosed();

            List<SqliteParameter> sqliteParameters = new()
            {
                new SqliteParameter("@Key", bookId), new SqliteParameter("@localKey", localTempId), new SqliteParameter("@userId", userId),
            };

            await SqliteFunctions.RunSqliteCommand("update BOOK set ID = @Key,LOCAL_TEMP_ID = null where LOCAL_TEMP_ID = @localKey and UID = @userId", sqliteParameters);

            SqliteFunctions.CloseIfOpen();
        }

        public static async Task AddBook(Book book, int? userId)
        {
            SqliteFunctions.OpenIfClosed();

            List<SqliteParameter> sqliteParametersList = new()
            {
                new SqliteParameter("@Id", book.Id),
                new SqliteParameter("@LocalTempId", book.LocalTempId),
                new SqliteParameter("@UserId", userId),
                new SqliteParameter("@Title", book.Title),
                new SqliteParameter("@SubTitle", book.SubTitle),
                new SqliteParameter("@Authors", book.Authors),
                new SqliteParameter("@Year", book.Year),
                new SqliteParameter("@Volume", book.Volume),
                new SqliteParameter("@Pages", book.Pages),
                new SqliteParameter("@Genre", book.Genre),
                new SqliteParameter("@UpdatedAt", book.UpdatedAt),
                new SqliteParameter("@Isbn", book.Isbn),
                new SqliteParameter("@Status", (int?)book.Status),
                new SqliteParameter("@Cover", book.Cover),
                new SqliteParameter("@GoogleId", book.GoogleId),
                new SqliteParameter("@Score", book.Score),
                new SqliteParameter("@Comment", book.Comment)
            };

            await SqliteFunctions.RunSqliteCommand("insert into BOOK(ID,LOCAL_TEMP_ID, UID, TITLE, SUBTITLE, AUTHORS, YEAR, VOLUME, PAGES, GENRE, UPDATED_AT, ISBN, STATUS,COVER,GOOGLE_ID,SCORE,COMMENT) " +
                "values (@Id, @LocalTempId, @UserId, @Title, @SubTitle, @Authors, @Year, @Volume, @Pages, @Genre, @UpdatedAt, @Isbn, @Status, @Cover, @GoogleId, @Score, @Comment)",
                sqliteParametersList);

            SqliteFunctions.CloseIfOpen();
        }

        /// <summary>
        /// update book local
        /// </summary>
        /// <param name="book"></param>
        public static async Task UpdateBook(Book book, int? userId)
        {
            SqliteFunctions.OpenIfClosed();

            List<SqliteParameter> sqliteParameters = new()
            {
                new SqliteParameter("@Id", book.Id),
                new SqliteParameter("@UserId", userId),
                new SqliteParameter("@Title", book.Title),
                new SqliteParameter("@SubTitle", book.SubTitle),
                new SqliteParameter("@Authors", book.Authors),
                new SqliteParameter("@Year", book.Year),
                new SqliteParameter("@Volume", book.Volume),
                new SqliteParameter("@Pages", book.Pages),
                new SqliteParameter("@Genre", book.Genre),
                new SqliteParameter("@LastUpdate", book.UpdatedAt),
                new SqliteParameter("@Isbn", book.Isbn),
                new SqliteParameter("@Inactive", book.Inactive),
                new SqliteParameter("@Situation", book.Status),
                new SqliteParameter("@Cover", book.Cover),
                new SqliteParameter("@GoogleId", book.GoogleId),
                new SqliteParameter("@Score", book.Score),
                new SqliteParameter("@Comment", book.Comment)
            };

            await SqliteFunctions.RunSqliteCommand("update BOOK set TITLE = @Title, SUBTITLE = @SubTitle, AUTHORS = @Authors, YEAR = @Year, VOLUME = @Volume, PAGES = @Pages" +
                ", GENRE = @Genre, UPDATED_AT = @LastUpdate,ISBN = @Isbn, INACTIVE = @Inactive, STATUS = @Situation, COVER = @Cover, GOOGLE_ID = @GoogleId, SCORE = @Score, COMMENT = @Comment" +
                " where ID = @Id and UID = @UserId",
                sqliteParameters);

            SqliteFunctions.CloseIfOpen();
        }

        public static DateTime? GetLastUpdateBook(int? id, string? title)
        {
            SqliteFunctions.OpenIfClosed();
            string command = "select UPDATED_AT from BOOK where";

            List<SqliteParameter> parameters = new();

            if (id != null)
            {
                command += " ID = @Key";
                parameters.Add(new SqliteParameter("@Key", id));
            }
            else
            {
                command += " TITLE = @Title";
                parameters.Add(new SqliteParameter("@Title", title));
            }

            SqliteDataReader response = Task.Run(async () => await SqliteFunctions.RunSqliteCommand(command, parameters)).Result;

            _ = response.Read();

            string? lastUpdate = "";

            if (response.HasRows)
            {
                lastUpdate = response.GetWithNullableString(0);
            }

            SqliteFunctions.CloseIfOpen();

            if (string.IsNullOrEmpty(lastUpdate)) return null;
            else return Convert.ToDateTime(lastUpdate);
        }

        /// <summary>
        /// Update or create local book with the newest version of the book in the server
        /// </summary>
        /// <param name="book"></param>
        public static async Task AddOrUpdateBook(Book book, int? userId)
        {
            DateTime? lastUpdate = GetLastUpdateBook(book?.Id, book?.Title);

            if (lastUpdate == null && book is not null && book.Inactive == 0)
                await AddBook(book, userId);
            else if (book is not null && book.UpdatedAt > lastUpdate)
               await UpdateBook(book, userId);
        }

        public async static Task<List<(Status, int)>> GetBookshelfTotals(int userId)
        {
            SqliteFunctions.OpenIfClosed();

            List<(Status, int)> booksTotalSituations = new();

            SqliteDataReader response = await SqliteFunctions.RunSqliteCommand("select STATUS,count(STATUS) from BOOK where UID = @UserId and (Inactive is null or Inactive = '0')  group by STATUS",
                new List<SqliteParameter>() { new SqliteParameter("@UserId", userId) });

            while (response.Read())
            {
                booksTotalSituations.Add(((Status)response.GetInt32(0), response.GetInt32(1)));
            }

            SqliteFunctions.CloseIfOpen();

            return booksTotalSituations;
        }

        public async static Task<Book> GetBook(int userId, string bookKey)
        {
            try
            {
                SqliteFunctions.OpenIfClosed();

                List<SqliteParameter> parameters = new()
                {
                    new SqliteParameter("@userId", userId),
                    new SqliteParameter("@key", bookKey)
                };

                SqliteDataReader response = await SqliteFunctions.RunSqliteCommand("select ID, TITLE, SUBTITLE, AUTHORS, YEAR, VOLUME, PAGES, ISBN, GENRE, UPDATED_AT, INACTIVE, STATUS," +
                    " COVER, GOOGLE_ID, SCORE, COMMENT, CREATED_AT from BOOK where UID = @userId and ID = @key", parameters);

                response.Read();

                Book book = new()
                {
                    Id = response.GetInt32(0),
                    Title = response.GetWithNullableString(1),
                    SubTitle = response.GetWithNullableString(2),
                    Authors = response.GetWithNullableString(3),
                    Year = response.GetWithNullableInt(4),
                    Volume = response.GetWithNullableInt(5),
                    Pages = response.GetInt32(6),
                    Isbn = response.GetWithNullableString(7),
                    Genre = response.GetWithNullableString(8),
                    UpdatedAt = Convert.ToDateTime(response.GetWithNullableString(9)),
                    Inactive = response.GetWithNullableInt(10),
                    Status = (Status)response.GetInt32(11),
                    Cover = response.GetWithNullableString(12),
                    GoogleId = response.GetWithNullableString(13),
                    Score = response.GetWithNullableInt(14),
                    Comment = response.GetWithNullableString(15),
                    CreatedAt = Convert.ToDateTime(response.GetWithNullableString(16)),
                };

                SqliteFunctions.CloseIfOpen();

                return book;
            }
            catch (Exception ex) { throw ex; }
        }

        public async static Task<Book?> GetBookByTitleOrGooglekey(int userId, string bookTitle, string? googleKey)
        {
            try
            {
                SqliteFunctions.OpenIfClosed();

                List<SqliteParameter> parameters = new()
                {
                    new SqliteParameter("@UserId", userId),
                    new SqliteParameter("@Title", bookTitle)
                };

                StringBuilder query = new();

                query.Append("select ID, TITLE, SUBTITLE, AUTHORS, YEAR, VOLUME, PAGES, ISBN, GENRE, UPDATED_AT, INACTIVE, STATUS, COVER, GOOGLE_ID, SCORE, COMMENT, CREATED_AT from BOOK where UID = @UserId and LOWER(TITLE) =  @Title");


                if (!string.IsNullOrEmpty(googleKey))
                {
                    parameters.Add(new SqliteParameter("@GoogleId", googleKey));

                    query.Append(" or GOOGLE_ID = @GoogleId");
                }

                SqliteDataReader response = await SqliteFunctions.RunSqliteCommand(query.ToString(), parameters);

                response.Read();

                Book? book = null;

                if (response.HasRows)
                {
                    book = new()
                    {
                        Id = response.GetInt32(0),
                        Title = response.GetWithNullableString(1),
                        SubTitle = response.GetWithNullableString(2),
                        Authors = response.GetWithNullableString(3),
                        Year = response.GetWithNullableInt(4),
                        Volume = response.GetWithNullableInt(5),
                        Pages = response.GetInt32(6),
                        Isbn = response.GetWithNullableString(7),
                        Genre = response.GetWithNullableString(8),
                        UpdatedAt = Convert.ToDateTime(response.GetWithNullableString(9)),
                        Inactive = response.GetWithNullableInt(10),
                        Status = (Status)response.GetInt32(11),
                        Cover = response.GetWithNullableString(12),
                        GoogleId = response.GetWithNullableString(13),
                        Score = response.GetWithNullableInt(14),
                        Comment = response.GetWithNullableString(15),
                        CreatedAt = Convert.ToDateTime(response.GetWithNullableString(16)),
                    };
                }

                SqliteFunctions.CloseIfOpen();

                return book;

            }
            catch (Exception ex) { throw ex; }
        }

        public static async Task<List<Book>> GetBookSituationByStatus(int Situation, int UserId, string? textoBusca)
        {
            try
            {
                SqliteFunctions.OpenIfClosed();

                string command = "select ID, TITLE, SUBTITLE, AUTHORS, YEAR, VOLUME, PAGES, ISBN, GENRE, UPDATED_AT, INACTIVE, STATUS, COVER, GOOGLE_ID, SCORE, COMMENT, CREATED_AT from BOOK where UID = @userId";

                if (Situation > 0)
                {
                    command += " and STATUS = @situation";
                }

                if (!string.IsNullOrEmpty(textoBusca))
                {
                    command += " and LOWER(TITLE) like @textoBusca";
                }

                command += " and (INACTIVE is null or INACTIVE = '0') order by UPDATED_AT desc";

                List<SqliteParameter> parameters = new()
                {
                    new SqliteParameter("@userId", UserId)
                };

                if (Situation > 0)
                {
                    parameters.Add(new SqliteParameter("@situation", Situation));
                }

                if (!string.IsNullOrEmpty(textoBusca))
                {
                    parameters.Add(new SqliteParameter("@textoBusca", "%" + textoBusca + "%"));
                }

                SqliteDataReader response = await SqliteFunctions.RunSqliteCommand(command, parameters);

                List<Book> lista = new();

                while (response.Read())
                {
                    lista.Add(new Book()
                    {
                        Id = response.GetInt32(0),
                        Title = response.GetWithNullableString(1),
                        SubTitle = response.GetWithNullableString(2),
                        Authors = response.GetWithNullableString(3),
                        Year = response.GetWithNullableInt(4),
                        Volume = response.GetWithNullableInt(5),
                        Pages = response.GetInt32(6),
                        Isbn = response.GetWithNullableString(7),
                        Genre = response.GetWithNullableString(8),
                        UpdatedAt = Convert.ToDateTime(response.GetWithNullableString(9)),
                        Inactive = response.GetWithNullableInt(10),
                        Status = (Status)response.GetInt32(11),
                        Cover = response.GetWithNullableString(12),
                        GoogleId = response.GetWithNullableString(13),
                        Score = response.GetWithNullableInt(14),
                        Comment = response.GetWithNullableString(15),
                        CreatedAt = Convert.ToDateTime(response.GetWithNullableString(16)),
                    });
                }

                SqliteFunctions.CloseIfOpen();

                return lista;
            }
            catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Inactivate a book in local batabase
        /// </summary>
        public static async Task InactivateBook(int? bookId, int userId, DateTime lastUpdate)
        {
            SqliteFunctions.OpenIfClosed();

            List<SqliteParameter> sqliteParameters = new()
            {
                new SqliteParameter("@Key", bookId),
                new SqliteParameter("@UserId", userId),
                new SqliteParameter("@LastUpdate", lastUpdate)
            };

            await SqliteFunctions.RunSqliteCommand("update BOOK set Inactive = '1',UPDATED_AT = @LastUpdate where ID = @Key and UID = @UserId",
                 sqliteParameters);

            SqliteFunctions.CloseIfOpen();
        }
    }
}
