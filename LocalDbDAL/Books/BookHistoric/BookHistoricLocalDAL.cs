using Microsoft.Data.Sqlite;
using Models;
using Models.Books;
using Models.Books.Historic;

namespace LocalDbDAL.Books.BookHistoric
{
    public class BookHistoricLocalDAL : IBookHistoricLocalDAL
    {
        public async Task<bool> CheckBookHistoricById(int? id)
        {
            SqliteFunctions.OpenIfClosed();

            string command = "select ID from BOOK_HISTORIC where ID = @Id";

            List<SqliteParameter> parameters = new()
            {
                new SqliteParameter("@Id", id)
            };

            SqliteDataReader response = await SqliteFunctions.RunSqliteCommand(command, parameters);

            response.Read();

            bool bookHistoricExists = false;

            if (response.HasRows)
                bookHistoricExists = true;

            SqliteFunctions.CloseIfOpen();

            return bookHistoricExists;

        }

        public async Task AddBookHistoric(Models.Books.Historic.BookHistoric bookHistoric, int? userId)
        {
            SqliteFunctions.OpenIfClosed();

            List<SqliteParameter> sqliteParametersList = new()
            {
                new SqliteParameter("@Id", bookHistoric.Id),
                new SqliteParameter("@CreatedAt", bookHistoric.CreatedAt),
                new SqliteParameter("@BookId", bookHistoric.BookId),
                new SqliteParameter("@UserId", userId),
                new SqliteParameter("@TypeId", bookHistoric.TypeId),
                new SqliteParameter("@Type", bookHistoric.Type),
            };

            await SqliteFunctions.RunSqliteCommand("insert into BOOK_HISTORIC(ID, CREATED_AT, BOOK_ID, TYPE_ID, TYPE, UID)" +
                " values (@Id, @CreatedAt, @BookId, @TypeId, @Type, @UserId)",
                sqliteParametersList);

            if (bookHistoric.BookHistoricItems != null)
                foreach (Models.Books.Historic.BookHistoricItem bookHistoricItem in bookHistoric.BookHistoricItems)
                {
                    SqliteFunctions.OpenIfClosed();

                    sqliteParametersList = new() {
                        new SqliteParameter("@Id", bookHistoricItem.Id),
                        new SqliteParameter("@CreatedAt", bookHistoricItem.CreatedAt),
                        new SqliteParameter("@BookFieldName", bookHistoricItem.BookFieldName),
                        new SqliteParameter("@UpdatedFrom", bookHistoricItem.UpdatedFrom),
                        new SqliteParameter("@UpdatedTo", bookHistoricItem.UpdatedTo),
                        new SqliteParameter("@UserId", userId),
                        new SqliteParameter("@BookHistoricId", bookHistoric.Id),
                        new SqliteParameter("@BookFieldId", bookHistoricItem.BookFieldId),
                    };

                    await SqliteFunctions.RunSqliteCommand("insert into BOOK_HISTORIC_ITEM(ID, CREATED_AT, BOOK_FIELD_ID, BOOK_FIELD_NAME, UPDATED_FROM, UPDATED_TO, UID, BOOK_HISTORIC_ID)" +
                        " values (@Id, @CreatedAt, @BookFieldId, @BookFieldName, @UpdatedFrom, @UpdatedTo, @UserId, @BookHistoricId)",
                        sqliteParametersList);
                }

            SqliteFunctions.CloseIfOpen();
        }

        public async Task<List<Models.Books.Historic.BookHistoric>> GetBookHistoricByBookId(int uid, int bookId)
        {
            try
            {
                List<SqliteParameter> parameters = new()
                {
                    new SqliteParameter("@BookId", bookId),
                    new SqliteParameter("@Uid", uid)
                };

                string command = "select h.id as historic_id, h.created_at as hist_created_at, h.book_id," +
                    " h.type_id, h.TYPE as type_name, i.id as item_id, i.updated_from, i.updated_to," +
                    " i.book_historic_id, i.created_at as item_created_at, i.BOOK_FIELD_NAME as field_name,i.BOOK_FIELD_ID as field_id" +
                    " from book_historic h left join book_historic_item i on h.id = i.book_historic_id where" +
                    " h.uid = @Uid and h.book_id = @BookId order by hist_created_at desc";

                SqliteFunctions.OpenIfClosed();

                SqliteDataReader response = await SqliteFunctions.RunSqliteCommand(command, parameters);

                List<Models.Books.Historic.BookHistoric> list = new();

                int counter = 0;

                while (response.Read())
                {
                    int bookHistoricId = response.GetInt32(0);
                    int? itemId = response.GetWithNullableInt(5);

                    if (counter > 0 && list.Count > 0 && list.Last().Id == bookHistoricId)
                    {

                        if (itemId is not null)
                        {
                            list.Last().BookHistoricItems?.Add(new BookHistoricItem()
                            {
                                Id = itemId,
                                BookFieldName = response.GetWithNullableString(10),
                                CreatedAt = Convert.ToDateTime(response.GetWithNullableString(9)),
                                UpdatedFrom = response.GetWithNullableString(6),
                                UpdatedTo = response.GetWithNullableString(7),
                                BookFieldId = response.GetInt32(11),
                            });
                        }
                    }
                    else
                    {
                        var bookHistoricRow = new Models.Books.Historic.BookHistoric()
                        {
                            Id = bookHistoricId,
                            TypeId = response.GetInt32(3),
                            Type = response.GetWithNullableString(4),
                            CreatedAt = Convert.ToDateTime(response.GetWithNullableString(1)),
                            BookId = bookId,
                            BookHistoricItems = new()
                        };

                        if (itemId is not null)
                        {
                            bookHistoricRow.BookHistoricItems?.Add(new BookHistoricItem()
                            {
                                Id = itemId,
                                BookFieldName = response.GetWithNullableString(10),
                                CreatedAt = Convert.ToDateTime(response.GetWithNullableString(9)),
                                UpdatedFrom = response.GetWithNullableString(6),
                                UpdatedTo = response.GetWithNullableString(7),
                                BookFieldId = response.GetInt32(11),
                            });
                        }

                        list.Add(bookHistoricRow);
                    }
                    counter++;
                }

                return list;
            }
            catch (Exception ex) { throw ex; }
        }
    }
}
