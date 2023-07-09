using Microsoft.Data.Sqlite;

namespace BookshelfRepos.User
{
    public static class UserRepos
    {

        public static void InsertUser(BookshelfModels.User.User user)
        {
            SQLiteDB.OpenIfClosed();

            List<SqliteParameter> parameters = new()
            {
                new SqliteParameter("@TOKEN", user.Token),
                new SqliteParameter("@NAME", user.Name),
                new SqliteParameter("@EMAIL", user.Email),
                new SqliteParameter("@PASSWORD", user.Password),
                new SqliteParameter("@LASTUPDATE", DateTime.MinValue),
            };

            _ = SQLiteDB.RunSqliteCommand("delete from USER");

            _ = SQLiteDB.RunSqliteCommand("insert into USER (TOKEN,NAME,EMAIL,PASSWORD,LASTUPDATE) values (@TOKEN,@NAME,@EMAIL,@PASSWORD,@LASTUPDATE)", parameters);

            SQLiteDB.CloseIfOpen();
        }

        public async static Task CleanUserDatabase()
        {
            SQLiteDB.OpenIfClosed();

            //clean local database
            await SQLiteDB.RunSqliteCommand("delete from USER");

            await SQLiteDB.RunSqliteCommand("delete from BOOK");

            SQLiteDB.CloseIfOpen();
        }

        public static async Task<BookshelfModels.User.User?> GetUser()
        {
            SQLiteDB.OpenIfClosed();

            SqliteDataReader ret = await SQLiteDB.RunSqliteCommand("select Id,token,email,password,lastUpdate from USER");
            ret.Read();

            if (ret.HasRows)
            {
                BookshelfModels.User.User user = new()
                {
                    Id = ret.GetWithNullableString(0),
                    Token = ret.GetWithNullableString(1),
                    Email = ret.GetWithNullableString(2),
                    Password = ret.GetWithNullableString(3),
                    LastUpdate = ret.GetDateTime(4),
                };

                SQLiteDB.CloseIfOpen();

                return user;
            }
            else
            {
                SQLiteDB.CloseIfOpen();
                return null;
            }
        }

        public static async Task UpdateUserLastUpdateLocal(string? id, DateTime lastUpdate)
        {
            SQLiteDB.OpenIfClosed();

            List<SqliteParameter> parameters = new() { new SqliteParameter("@Id", id), new SqliteParameter("@LastUpdate", lastUpdate) };

            await SQLiteDB.RunSqliteCommand("update USER set LastUpdate = @LastUpdate where Id = @Id", parameters);

            SQLiteDB.CloseIfOpen();
        }

        public static async Task UpdateToken(string? Key, string? token)
        {
            try
            {
                SQLiteDB.OpenIfClosed();

                List<SqliteParameter> parameters = new() { new SqliteParameter("@Id", Key), new SqliteParameter("@token", token) };

                await SQLiteDB.RunSqliteCommand("update USER set token = @token where Id = @Id", parameters);

                SQLiteDB.CloseIfOpen();
            }
            catch (Exception) { throw; }
        }
    }
}
