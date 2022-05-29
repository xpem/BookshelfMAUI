using Microsoft.Data.Sqlite;

namespace BookshelfRepos.User
{
    public static class UserRepos
    {
        //public void AddUserLocal(BookshelfModels.User.User user)
        //{
        //    OpenIfClosed();

        //    List<SqliteParameter> parameters = new List<SqliteParameter>()
        //    {
        //        new SqliteParameter("@TOKEN", user.Token), new SqliteParameter("@EMAIL", user.Email),
        //        new SqliteParameter("@PASSWORD", user.Password), new SqliteParameter("@LASTUPDATE", DateTime.MinValue),
        //    };

        //    _ = RunSqliteCommand("insert into USER (TOKEN,EMAIL,PASSWORD,LASTUPDATE) values (@TOKEN,@EMAIL,@PASSWORD,@LASTUPDATE)", parameters);

        //    CloseIfOpen();

        //    ExecuteSQLiteCommand("insert into USER (TOKEN,EMAIL,PASSWORD,LASTUPDATE) values (?,?,?,?)", new object[] { user.Token, user.Email, user.Password, DateTime.MinValue });


        //}

        //public async void DelUserLocal()
        //{
        //    OpenIfClosed();

        //    //clean local database
        //    await RunSqliteCommand("delete from USER");
        //    await RunSqliteCommand("delete from BOOK");
        //    await RunSqliteCommand("delete from BOOKRATING");

        //    CloseIfOpen();
        //}

        public static BookshelfModels.User.User? GetUser()
        {
            SQLiteDB.OpenIfClosed();

            SqliteDataReader ret = Task.Run(async () => await SQLiteDB.RunSqliteCommand("select Id,token,email,password,lastUpdate from USER")).Result;
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

        public static void UpdateUserLastUpdateLocal(string? id, DateTime lastUpdate)
        {
            SQLiteDB.OpenIfClosed();

            List<SqliteParameter> parameters = new() { new SqliteParameter("@Id", id), new SqliteParameter("@LastUpdate", lastUpdate) };

            _ = SQLiteDB.RunSqliteCommand("update USER set LastUpdate = @LastUpdate where Id = @Id", parameters);

            SQLiteDB.CloseIfOpen();
        }

        public static void UpdateToken(string? Key, string? token)
        {
            try
            {
                SQLiteDB.OpenIfClosed();

                List<SqliteParameter> parameters = new() { new SqliteParameter("@Id", Key), new SqliteParameter("@token", token) };

                _ = SQLiteDB.RunSqliteCommand("update USER set token = @token where Id = @Id", parameters);

                SQLiteDB.CloseIfOpen();
            }
            catch (Exception ex) { throw ex; }
        }
    }
}
