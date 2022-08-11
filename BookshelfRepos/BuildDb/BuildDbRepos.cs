using BookshelfModels;
using Microsoft.Data.Sqlite;

namespace BookshelfRepos.BuildDb
{
    public static class BuildDbRepos
    {
        /// <summary>
        /// create or update the structure of SQLite tables by actual version defined
        /// </summary>
        public static void BuildDb()
        {
            SQLiteDB.OpenIfClosed();

            UpdateSQLiteTablesByVersions();

            _ = SQLiteDB.RunSqliteCommand("create table if not exists USER (Id integer primary key autoincrement, EMAIL text, UID text,TOKEN text,Password text, lastUpdate datetime);");
            _ = SQLiteDB.RunSqliteCommand("create table if not exists BOOK (Key text, UserId text, Title text, SubTitle text, Authors text, Year integer, Volume text, Pages integer, Isbn text, Genre text, LastUpdate DATETIME,Inactive integer,Situation integer,Cover TEXT,GoogleId TEXT);");
            _ = SQLiteDB.RunSqliteCommand("create table if not exists BOOKRATING (BookKey text, Rate integer, Comment text);");
            _ = SQLiteDB.RunSqliteCommand("create table if not exists VERSIONDB (USER integer,BOOK integer);");

            SQLiteDB.CloseIfOpen();
        }

        /// <summary>
        /// delete tables of old versions and recreate it
        /// </summary>
        private static async void UpdateSQLiteTablesByVersions()
        {

            _ = SQLiteDB.RunSqliteCommand("create table if not exists VERSIONSTABLES (Key integer, USER integer,BOOK integer);");

            VersionsDbTables versionsDbTables;

            using (SqliteDataReader Retorno = await SQLiteDB.RunSqliteCommand("select USER,BOOK from VERSIONSTABLES"))
            {
                _ = Retorno.Read();

                if (Retorno.HasRows)
                {
                    versionsDbTables = new VersionsDbTables()
                    {
                        USER = Retorno.GetWithNullableInt(0),
                        BOOK = Retorno.GetWithNullableInt(1)
                    };

                }
                else
                {
                    versionsDbTables = new VersionsDbTables()
                    {
                        USER = 0,
                        BOOK = 0
                    };
                    AddorUpdateVersionDb(false, versionsDbTables);
                }
            }

            bool updateVersionDb = false;

            if ((versionsDbTables.BOOK < SQLiteDB.ActualVersionsDbTables.BOOK) || (versionsDbTables.USER < SQLiteDB.ActualVersionsDbTables.USER))
            {
                _ = SQLiteDB.RunSqliteCommand("drop table if exists USER");

                updateVersionDb = true;
            }
            if (versionsDbTables.BOOK < SQLiteDB.ActualVersionsDbTables.BOOK)
            {
                _ = SQLiteDB.RunSqliteCommand("drop table if exists BOOK");
                _ = SQLiteDB.RunSqliteCommand("drop table if exists BOOKRATING");

                updateVersionDb = true;
            }

            if (updateVersionDb)
                AddorUpdateVersionDb(true, SQLiteDB.ActualVersionsDbTables);
        }

        private static void AddorUpdateVersionDb(bool isUpdate, VersionsDbTables versionsDbTables)
        {
            string command;

            if (!isUpdate)
            {
                command = "insert into VERSIONSTABLES(key,USER,BOOK) values ('1',@USER,@BOOK)";
            }
            else
            {
                command = "update VERSIONSTABLES set USER = @USER,BOOK = @BOOK where key = '1'";
            }

            List<SqliteParameter> parameters = new()
            {
                new SqliteParameter("@USER", versionsDbTables.USER),
                new SqliteParameter("@BOOK", versionsDbTables.BOOK)
            };

            _ = SQLiteDB.RunSqliteCommand(command, parameters);
        }

    }
}
