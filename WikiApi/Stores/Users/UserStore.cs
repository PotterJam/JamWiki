using System;
using System.Data.Common;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Npgsql;
using NpgsqlTypes;
using WikiApi.Stores.User;

namespace WikiApi.Stores.Users
{
    public class UserStore : IUserStore
    {
        private readonly NpgsqlConnection _dbConnection;
        
        public UserStore(PostgresConfiguration postgresConfiguration)
        {
            var dbConnectionStringBuilder = new NpgsqlConnectionStringBuilder("Server=localhost;Port=5433;UserId=jamwikiapp;Database=wikiapi;");
            dbConnectionStringBuilder.Add("Password", postgresConfiguration.Password);
            var dbConStr = dbConnectionStringBuilder.ToString();
            
            _dbConnection = new NpgsqlConnection(dbConStr);
            _dbConnection.Open();
        }

        ~UserStore()
        {
            _dbConnection.Close();
        }

        public async Task<WikiUser> GetUser(GoogleJsonWebSignature.Payload payload)
        {
            var cmd = new NpgsqlCommand(@"SELECT *
                                                   FROM users
                                                   WHERE email = @email", _dbConnection);
            cmd.Parameters.AddWithValue("email", NpgsqlDbType.Text, payload.Email);
            
            DbDataReader reader = await cmd.ExecuteReaderAsync();

            if (!reader.Read())
            {
                // user hasn't got an account
                reader.Close();
                return await CreateUser(payload);
            }

            return new WikiUser
            {
                Id = (Guid) reader["id"],
                Name = (string) reader["name"],
                Email = (string) reader["email"],
                Subject = (string) reader["subject"],
                Issuer = (string) reader["issuer"]
            };
        }

        private async Task<WikiUser> CreateUser(GoogleJsonWebSignature.Payload payload)
        {
            var user = new WikiUser
            {
                Id = Guid.NewGuid(),
                Name = payload.Name,
                Email = payload.Email,
                Subject = payload.Subject,
                Issuer = payload.Issuer
            };

            NpgsqlCommand cmd = new NpgsqlCommand(@"INSERT INTO users(id, name, email, subject, issuer)
                                                             VALUES (@id, @name, @email, @subject, @issuer)", _dbConnection);
            cmd.Parameters.AddWithValue("id", NpgsqlDbType.Uuid, user.Id);
            cmd.Parameters.AddWithValue("name", NpgsqlDbType.Text, user.Name);
            cmd.Parameters.AddWithValue("email", NpgsqlDbType.Text, user.Email);
            cmd.Parameters.AddWithValue("subject", NpgsqlDbType.Text, user.Subject);
            cmd.Parameters.AddWithValue("issuer", NpgsqlDbType.Text, user.Issuer);

            var rowsChanged = await cmd.ExecuteNonQueryAsync();
            if (rowsChanged != 1)
            {
                throw new NpgsqlException("Added more than one user, something has gone seriously wrong. ");
            }
            
            return user;
        }
    }
}