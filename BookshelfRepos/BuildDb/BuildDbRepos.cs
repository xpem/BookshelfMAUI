using BookshelfModels;
using Microsoft.Data.Sqlite;

namespace BookshelfRepos.BuildDb
{
    public static class BuildDbRepos
    {
        /// <summary>
        /// create or update the structure of SQLite tables by actual version defined
        /// </summary>
        public static async Task BuildDb()
        {
            SQLiteDB.OpenIfClosed();

            await UpdateSQLiteTablesByVersions();

            await SQLiteDB.RunSqliteCommand("create table if not exists USER (ID integer primary key autoincrement,NAME text, EMAIL text, UID text, TOKEN text,PASSWORD text, LASTUPDATE datetime);");
            await SQLiteDB.RunSqliteCommand("create table if not exists BOOK (ID integer,LOCAL_TEMP_ID text, UID text, TITLE text, SUBTITLE text, AUTHORS text, " +
                "YEAR integer, VOLUME text, PAGES integer, ISBN text, GENRE text, UPDATED_AT datetime, INACTIVE integer, STATUS integer," +
                " COVER text, GOOGLE_ID text, SCORE integer, COMMENT text, CREATED_AT datetime);");
            await SQLiteDB.RunSqliteCommand("create table if not exists VERSIONDB (USER integer, BOOK integer);");

            SQLiteDB.CloseIfOpen();
        }

        /// <summary>
        /// delete tables of old versions and recreate it
        /// </summary>
        private static async Task UpdateSQLiteTablesByVersions()
        {

            await SQLiteDB.RunSqliteCommand("create table if not exists VERSIONSTABLES (Key integer, USER integer,BOOK integer);");

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
                    await AddorUpdateVersionDb(false, versionsDbTables);
                }
            }

            bool updateVersionDb = false;

            if ((versionsDbTables.BOOK < SQLiteDB.ActualVersionsDbTables.BOOK) || (versionsDbTables.USER < SQLiteDB.ActualVersionsDbTables.USER))
            {
                await SQLiteDB.RunSqliteCommand("drop table if exists USER");

                updateVersionDb = true;
            }
            if (versionsDbTables.BOOK < SQLiteDB.ActualVersionsDbTables.BOOK)
            {
                await SQLiteDB.RunSqliteCommand("drop table if exists BOOK");

                updateVersionDb = true;
            }

            if (updateVersionDb)
                await AddorUpdateVersionDb(true, SQLiteDB.ActualVersionsDbTables);
        }

        private static async Task AddorUpdateVersionDb(bool isUpdate, VersionsDbTables versionsDbTables)
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

            await SQLiteDB.RunSqliteCommand(command, parameters);
        }

    }
}
