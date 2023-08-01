using Microsoft.Data.Sqlite;
using Models;

namespace LocalDbDAL.BuildDb
{
    public static class BuildLocalDbDAL
    {
        /// <summary>
        /// create or update the structure of SQLite tables by actual version defined
        /// </summary>
        public static async Task BuildDb()
        {
            SqliteFunctions.OpenIfClosed();

            await CreateTableVersions();

            await UpdateSQLiteTablesByVersions();

            await SqliteFunctions.RunSqliteCommand("create table if not exists USER (ID integer primary key autoincrement,NAME text, EMAIL text, UID text, TOKEN text,PASSWORD text, LASTUPDATE datetime);");

            await SqliteFunctions.RunSqliteCommand("create table if not exists BOOK (ID integer,LOCAL_TEMP_ID text, UID text, TITLE text, SUBTITLE text, AUTHORS text, " +
                "YEAR integer, VOLUME text, PAGES integer, ISBN text, GENRE text, UPDATED_AT datetime, INACTIVE integer, STATUS integer," +
                " COVER text, GOOGLE_ID text, SCORE integer, COMMENT text, CREATED_AT datetime);");

            await SqliteFunctions.RunSqliteCommand("create table if not exists BOOK_HISTORIC(ID integer, CREATED_AT datetime, BOOK_ID integer, TYPE_ID integer, TYPE text, UID text);");

            await SqliteFunctions.RunSqliteCommand("create table if not exists BOOK_HISTORIC_ITEM(ID integer, CREATED_AT datetime, BOOK_FIELD_NAME text, UPDATED_FROM integer," +
                " UPDATED_TO text, UID text, BOOK_HISTORIC_ID integer);");

            SqliteFunctions.CloseIfOpen();
        }

        private static async Task CreateTableVersions()
        {
            await SqliteFunctions.RunSqliteCommand("create table if not exists TBVERSIONS (TBVERSIONS integer, USER integer, BOOK integer, BOOK_HISTORIC integer, BOOK_HISTORIC_ITEM integer);");
        }

        /// <summary>
        /// delete tables of old versions and recreate it
        /// </summary>
        private static async Task UpdateSQLiteTablesByVersions()
        {
            await VerifyVersionDBTable();

            VersionsDbTables versionsDbTables;

            using (SqliteDataReader Retorno = await SqliteFunctions.RunSqliteCommand("select USER, BOOK, BOOK_HISTORIC, BOOK_HISTORIC_ITEM from TBVERSIONS"))
            {
                Retorno.Read();

                if (Retorno.HasRows)
                {
                    versionsDbTables = new VersionsDbTables()
                    {
                        USER = Retorno.GetWithNullableInt(0),
                        BOOK = Retorno.GetWithNullableInt(1),
                        BOOK_HISTORIC = Retorno.GetWithNullableInt(2),
                        BOOK_HISTORIC_ITEM = Retorno.GetWithNullableInt(3)
                    };
                }
                else
                {
                    versionsDbTables = new VersionsDbTables()
                    {
                        VERSION = 0,
                        USER = 0,
                        BOOK = 0,
                        BOOK_HISTORIC = 0,
                        BOOK_HISTORIC_ITEM = 0
                    };

                    await AddVersionDb();
                }
            }

            await DropOldVersionTables(versionsDbTables);
        }

        private static async Task VerifyVersionDBTable()
        {
            int version = 0;

            //TO FORCE UPDATE VERSIONTABLES table
            using (SqliteDataReader Retorno = await SqliteFunctions.RunSqliteCommand("select TBVERSIONS from TBVERSIONS"))
            {
                Retorno.Read();

                if (Retorno.HasRows)
                    version = Retorno.GetWithNullableInt(0) ?? 0;
            }

            if (version < SqliteFunctions.ActualVersionsDbTables.VERSION)
            {
                await SqliteFunctions.RunSqliteCommand("drop table if exists TBVERSIONS");
                await CreateTableVersions();
            }
        }

        private static async Task DropOldVersionTables(VersionsDbTables versionsDbTables)
        {
            bool updateVersionDb = false;

            if ((versionsDbTables.USER < SqliteFunctions.ActualVersionsDbTables.USER))
            {
                await SqliteFunctions.RunSqliteCommand("drop table if exists USER");

                updateVersionDb = true;
            }

            if (versionsDbTables.BOOK < SqliteFunctions.ActualVersionsDbTables.BOOK)
            {
                await SqliteFunctions.RunSqliteCommand("drop table if exists BOOK");

                updateVersionDb = true;
            }

            if (versionsDbTables.BOOK_HISTORIC < SqliteFunctions.ActualVersionsDbTables.BOOK_HISTORIC)
            {
                await SqliteFunctions.RunSqliteCommand("drop table if exists BOOK_HISTORIC");

                updateVersionDb = true;
            }

            if (versionsDbTables.BOOK_HISTORIC_ITEM < SqliteFunctions.ActualVersionsDbTables.BOOK_HISTORIC_ITEM)
            {
                await SqliteFunctions.RunSqliteCommand("drop table if exists BOOK_HISTORIC_ITEM");

                updateVersionDb = true;
            }

            if (updateVersionDb)
                await UpdateVersionDb(SqliteFunctions.ActualVersionsDbTables);
        }


        private static async Task AddVersionDb()
        {
            await SqliteFunctions.RunSqliteCommand("insert into TBVERSIONS(USER,BOOK,BOOK_HISTORIC,BOOK_HISTORIC_ITEM,TBVERSIONS) values (0,0,0,0,0);");
        }

        private static async Task UpdateVersionDb(VersionsDbTables versionsDbTables)
        {
            string command = "update TBVERSIONS set USER = @USER, BOOK = @BOOK, BOOK_HISTORIC = @BOOK_HISTORIC, BOOK_HISTORIC_ITEM = @BOOK_HISTORIC_ITEM, TBVERSIONS = @TBVERSIONS";

            List<SqliteParameter> parameters = new()
            {
                new SqliteParameter("@USER", versionsDbTables.USER),
                new SqliteParameter("@BOOK", versionsDbTables.BOOK),
                new SqliteParameter("@BOOK_HISTORIC", versionsDbTables.BOOK_HISTORIC),
                new SqliteParameter("@BOOK_HISTORIC_ITEM", versionsDbTables.BOOK_HISTORIC_ITEM),
                new SqliteParameter("@TBVERSIONS", versionsDbTables.VERSION),
            };

            await SqliteFunctions.RunSqliteCommand(command, parameters);
        }

        public async static Task CleanDatabase()
        {
            SqliteFunctions.OpenIfClosed();

            //clean local database
            await SqliteFunctions.RunSqliteCommand("delete from USER");

            await SqliteFunctions.RunSqliteCommand("delete from BOOK");

            await SqliteFunctions.RunSqliteCommand("delete from BOOK_HISTORIC");

            await SqliteFunctions.RunSqliteCommand("delete from BOOK_HISTORIC_ITEM");

            SqliteFunctions.CloseIfOpen();
        }

    }
}
