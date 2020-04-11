using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JamWiki.Api.Config;
using JamWiki.Api.Users;
using Npgsql;
using NpgsqlTypes;

namespace JamWiki.Api.Wikis
{
    public class WikiStore : IWikiStore
    {
        private readonly string m_ConnectionString;

        public WikiStore(PostgresConfiguration postgresConfiguration)
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

        public async Task<Wiki> GetWikiByName(string wikiName, WikiUser wikiUser)
        {
            await using var conn = new NpgsqlConnection(m_ConnectionString);
            await conn.OpenAsync();

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT *
                                        FROM  wikis
                                        WHERE name    = @name
                                        AND   user_id = @user_id;";

            cmd.Parameters.AddWithValue("user_id", NpgsqlDbType.Uuid, wikiUser.Id);
            cmd.Parameters.AddWithValue("name", NpgsqlDbType.Text, wikiName);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (!reader.Read())
            {
                // no wiki found
                return null;
            }

            var wikiId = (Guid) reader["id"];
            var wikiBody = (string) reader["body"];

            var tagsFromReader = reader["tags"];
            var wikiTags = tagsFromReader is DBNull
                ? Enumerable.Empty<string>()
                : (string[]) tagsFromReader;

            return new Wiki(wikiId, wikiName, wikiBody, wikiTags);
        }

        public async Task<IEnumerable<Wiki>> GetWikisWithTag(string tag, WikiUser wikiUser)
        {
            await using var conn = new NpgsqlConnection(m_ConnectionString);
            await conn.OpenAsync();

            await using var cmd = conn.CreateCommand();

            cmd.CommandText = @"SELECT *
                                FROM wikis
                                WHERE @tag = ANY(tags)
                                AND user_id = @user_id;";
            
            cmd.Parameters.AddWithValue("user_id", NpgsqlDbType.Uuid, wikiUser.Id);
            cmd.Parameters.AddWithValue("tag", NpgsqlDbType.Text, tag);
            
            await using var reader = await cmd.ExecuteReaderAsync();

            var wikis = new List<Wiki>();

            while (reader.Read())
            {
                var wikiId = (Guid) reader["id"];
                var wikiName = (string) reader["name"];
                var wikiBody = (string) reader["body"];
                
                var tagsFromReader = reader["tags"];
                var wikiTags = tagsFromReader is DBNull ? Enumerable.Empty<string>() : (string[]) tagsFromReader;

                wikis.Add(new Wiki(wikiId, wikiName, wikiBody, wikiTags));
            }
            return wikis;
        }

        public async Task AddWiki(Wiki newWiki, WikiUser wikiUser)
        {
            if (string.IsNullOrWhiteSpace(newWiki.Name))
                return;

            await using var conn = new NpgsqlConnection(m_ConnectionString);
            await conn.OpenAsync();

            await using var cmd = conn.CreateCommand();

            cmd.CommandText = @"INSERT INTO wikis(id, name, body, tags, user_id)
                                VALUES (@id, @name, @body, @tags, @user_id);";
            
            cmd.Parameters.AddWithValue("id", NpgsqlDbType.Uuid, newWiki.Id);
            cmd.Parameters.AddWithValue("name", NpgsqlDbType.Text, newWiki.Name);
            cmd.Parameters.AddWithValue("body", NpgsqlDbType.Text, newWiki.Body);
            cmd.Parameters.AddWithValue("tags", NpgsqlDbType.Array | NpgsqlDbType.Text, newWiki.Tags);
            cmd.Parameters.AddWithValue("user_id", NpgsqlDbType.Uuid, wikiUser.Id);

            var rowsChanged = await cmd.ExecuteNonQueryAsync();
            if (rowsChanged == 0)
            {
                throw new NpgsqlException("Didn't add wiki, something went wrong.");
            }
            
            if (rowsChanged > 1)
            {
                throw new NpgsqlException("Added more than one wiki, something has gone seriously wrong. ");
            }
        }

        public async Task DeleteWikiByName(string name, WikiUser wikiUser)
        {
            await using var conn = new NpgsqlConnection(m_ConnectionString);
            await conn.OpenAsync();

            await using var cmd = conn.CreateCommand();
            
            cmd.CommandText = @"DELETE FROM wikis
                                WHERE name=@name
                                AND user_id=@user_id;";
            
            cmd.Parameters.AddWithValue("name", NpgsqlDbType.Text, name);
            cmd.Parameters.AddWithValue("user_id", NpgsqlDbType.Uuid, wikiUser.Id);

            var rowsChanged = await cmd.ExecuteNonQueryAsync();
            if (rowsChanged == 0)
            {
                throw new NpgsqlException("Didn't delete wiki, something went wrong.");
            }
            
            if (rowsChanged > 1)
            {
                throw new NpgsqlException("Deleted more than one wiki, something has gone seriously wrong. ");
            }
        }

        public async Task<IEnumerable<string>> GetWikiNames(WikiUser wikiUser)
        {
            await using var conn = new NpgsqlConnection(m_ConnectionString);
            await conn.OpenAsync();

            await using var cmd = conn.CreateCommand();
            
            cmd.CommandText = @"SELECT name
                                  FROM wikis
                                  WHERE user_id=@user_id;";
            
            cmd.Parameters.AddWithValue("user_id", NpgsqlDbType.Uuid, wikiUser.Id);
            
            await using var reader = await cmd.ExecuteReaderAsync();

            var wikiNames = new List<string>();
            while (reader.Read())
            {
                wikiNames.Add((string) reader["name"]);
            }

            return wikiNames;
        }

        public async Task UpdateWiki(Wiki updatedWiki, WikiUser wikiUser)
        {
            await using var conn = new NpgsqlConnection(m_ConnectionString);
            await conn.OpenAsync();

            await using var cmd = conn.CreateCommand();
            
            cmd.CommandText = @"UPDATE wikis
                                SET body = @body,
                                    tags = @tags,
                                    last_modified = now()
                                WHERE name = @name
                                AND user_id=@user_id;";
            
            cmd.Parameters.AddWithValue("name", NpgsqlDbType.Text, updatedWiki.Name);
            cmd.Parameters.AddWithValue("body", NpgsqlDbType.Text, updatedWiki.Body);
            cmd.Parameters.AddWithValue("tags", NpgsqlDbType.Array | NpgsqlDbType.Text, updatedWiki.Tags);
            cmd.Parameters.AddWithValue("user_id", NpgsqlDbType.Uuid, wikiUser.Id);
            
            var rowsChanged = await cmd.ExecuteNonQueryAsync();
            
            if (rowsChanged == 0)
            {
                throw new NpgsqlException("Didn't update wiki, something went wrong.");
            }
            
            if (rowsChanged > 1)
            {
                throw new NpgsqlException("Updated more than one wiki, something has gone seriously wrong. ");
            }
        }

        public async Task<IEnumerable<string>> GetAllWikiTags(WikiUser wikiUser)
        {
            await using var conn = new NpgsqlConnection(m_ConnectionString);
            await conn.OpenAsync();

            await using var cmd = conn.CreateCommand();
            
            cmd.CommandText = @"select array_agg(c) tags
                    FROM (
                        SELECT unnest(tags)
                        FROM wikis
                        WHERE user_id=@user_id
                    ) AS dt(c);";
            
            cmd.Parameters.AddWithValue("user_id", NpgsqlDbType.Uuid, wikiUser.Id);
            
            await using var reader = await cmd.ExecuteReaderAsync();

            if (!reader.Read())
            {
                return null;
            }
            
            var wikiTagsFromDb = reader["tags"];
            var wikiTags = wikiTagsFromDb is DBNull ? Enumerable.Empty<string>() : (string[]) wikiTagsFromDb;

            // TODO: handle casing
            return wikiTags.Distinct();
        }
    }
}