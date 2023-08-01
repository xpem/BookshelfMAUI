﻿using Microsoft.Data.Sqlite;
using Models;

namespace LocalDbDAL
{
    public static class SqliteFunctions
    {
        private static readonly SqliteConnection db = new($"Filename={Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Bookshelf.db")}");

        /// <summary>
        /// version upgrade of a table force his recreation
        /// </summary>
        public static readonly VersionsDbTables ActualVersionsDbTables = new() { VERSION = 1, USER = 4, BOOK = 21, BOOK_HISTORIC = 1, BOOK_HISTORIC_ITEM = 1 };

        /// <summary>
        /// Need additional package Microsoft.EntityFrameworkCore.Sqlite to work
        ///// </summary>
        public static void OpenIfClosed() { if (db.State == System.Data.ConnectionState.Closed) db.Open(); }

        public static void CloseIfOpen() { if (db.State == System.Data.ConnectionState.Open) db.Close(); }

        public async static Task<SqliteDataReader> RunSqliteCommand(string command, List<SqliteParameter>? parameters = null)
        {
            try
            {
                SqliteCommand sqliteCommand = new(command, db);

                if (parameters != null)
                    foreach (SqliteParameter parameter in parameters)
                    {
                        if (parameter.Value == null)
                            sqliteCommand.Parameters.AddWithValue(parameter.ParameterName, DBNull.Value);
                        else
                            sqliteCommand.Parameters.AddWithValue(parameter.ParameterName, parameter.Value);
                    }


                return await sqliteCommand.ExecuteReaderAsync();
            }catch (Exception ex) { throw ex; }
        }

        public static string? GetWithNullableString(this SqliteDataReader sqliteDataReader, int ordinal) => !sqliteDataReader.IsDBNull(ordinal) ? sqliteDataReader.GetString(ordinal) : null;

        public static bool GetWithNullableBool(this SqliteDataReader sqliteDataReader, int ordinal) => !sqliteDataReader.IsDBNull(ordinal) && sqliteDataReader.GetBoolean(ordinal);

        public static int? GetWithNullableInt(this SqliteDataReader sqliteDataReader, int ordinal) => !sqliteDataReader.IsDBNull(ordinal) ? sqliteDataReader.GetInt32(ordinal) : null;
    }
}
