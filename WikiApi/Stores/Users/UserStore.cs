using System;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Npgsql;
using NpgsqlTypes;
using WikiApi.Stores.User;

namespace WikiApi.Stores.Users
{
    public class UserStore : IUserStore
    {
        private readonly string m_ConnectionString;
        
        public UserStore(PostgresConfiguration postgresConfiguration)
        {
            var connectionStrBuilder = new NpgsqlConnectionStringBuilder
            {
                Database = postgresConfiguration.Database,
                Host = postgresConfiguration.Host,
                Port = postgresConfiguration.Port,
                Username = postgresConfiguration.Username,
                Password = postgresConfiguration.Password
            };
            
            m_ConnectionString = connectionStrBuilder.ToString();
        }
        
        public async Task<WikiUser> GetUser(GoogleJsonWebSignature.Payload payload)
        {
            await using var conn = new NpgsqlConnection(m_ConnectionString);
            await conn.OpenAsync();

            await using var cmd = conn.CreateCommand();
            
            cmd.CommandText = @"SELECT *
                                FROM users
                                WHERE email = @email;";
            
            cmd.Parameters.AddWithValue("email", NpgsqlDbType.Text, payload.Email);
            
            await using var reader = await cmd.ExecuteReaderAsync();

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

            await using var conn = new NpgsqlConnection(m_ConnectionString);
            await conn.OpenAsync();

            await using var cmd = conn.CreateCommand();
            
            cmd.CommandText = @"INSERT INTO users(id, name, email, subject, issuer)
                                 VALUES (@id, @name, @email, @subject, @issuer);";
            
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