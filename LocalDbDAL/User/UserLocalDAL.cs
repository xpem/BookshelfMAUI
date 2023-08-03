﻿using Microsoft.Data.Sqlite;

namespace LocalDbDAL.User
{
    public class UserLocalDAL : IUserLocalDAL
    {
        public async Task InsertUser(Models.User user)
        {
            SqliteFunctions.OpenIfClosed();

            await SqliteFunctions.RunSqliteCommand("delete from USER");

            List<SqliteParameter> parameters = new()
            {
                new SqliteParameter("@TOKEN", user.Token),
                new SqliteParameter("@NAME", user.Name),
                new SqliteParameter("@EMAIL", user.Email),
                new SqliteParameter("@PASSWORD", user.Password),
                new SqliteParameter("@LASTUPDATE", DateTime.MinValue),
            };

            await SqliteFunctions.RunSqliteCommand("insert into USER (TOKEN,NAME,EMAIL,PASSWORD,LASTUPDATE) values (@TOKEN,@NAME,@EMAIL,@PASSWORD,@LASTUPDATE)", parameters);

            SqliteFunctions.CloseIfOpen();
        }

        public async Task<Models.User?> GetUser()
        {
            SqliteFunctions.OpenIfClosed();

            SqliteDataReader ret = await SqliteFunctions.RunSqliteCommand("select Id,token,email,password,lastUpdate from USER");
            ret.Read();

            if (ret.HasRows)
            {
                Models.User user = new()
                {
                    Id = ret.GetInt32(0),
                    Token = ret.GetWithNullableString(1),
                    Email = ret.GetWithNullableString(2),
                    Password = ret.GetWithNullableString(3),
                    LastUpdate = ret.GetDateTime(4),
                };

                SqliteFunctions.CloseIfOpen();

                return user;
            }
            else
            {
                SqliteFunctions.CloseIfOpen();
                return null;
            }
        }

        public async Task UpdateUserLastUpdateLocal(int? id, DateTime lastUpdate)
        {
            SqliteFunctions.OpenIfClosed();

            List<SqliteParameter> parameters = new() { new SqliteParameter("@Id", id), new SqliteParameter("@LastUpdate", lastUpdate) };

            await SqliteFunctions.RunSqliteCommand("update USER set LastUpdate = @LastUpdate where Id = @Id", parameters);

            SqliteFunctions.CloseIfOpen();
        }

        public async Task UpdateToken(int? id, string? token)
        {
            try
            {
                SqliteFunctions.OpenIfClosed();

                List<SqliteParameter> parameters = new() { new SqliteParameter("@Id", id), new SqliteParameter("@token", token) };

                await SqliteFunctions.RunSqliteCommand("update USER set token = @token where Id = @Id", parameters);

                SqliteFunctions.CloseIfOpen();
            }
            catch (Exception) { throw; }
        }

        public async Task<string?> GetUserToken()
        {
            SqliteFunctions.OpenIfClosed();

            SqliteDataReader ret = await SqliteFunctions.RunSqliteCommand("select TOKEN from USER");

            ret.Read();
            if (ret.HasRows)
            {
                string token = ret.GetString(0);

                SqliteFunctions.CloseIfOpen();

                return token;
            }
            else
            {
                SqliteFunctions.CloseIfOpen();
                return null;
            }
        }
    }
}
