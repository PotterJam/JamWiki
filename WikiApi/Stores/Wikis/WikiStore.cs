using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;

namespace WikiApi.Stores.Wikis
{
    public class WikiStore : IWikiStore
    {
        private readonly NpgsqlConnection _dbConnection;
        
        public WikiStore(IConfiguration configuration)
        {
            var dbConnectionStringBuilder = new NpgsqlConnectionStringBuilder("Server=localhost;Port=5433;UserId=jamwikiapp;Database=wikiapi;");
            dbConnectionStringBuilder.Add("Password", configuration["DbPassword"]);
            var dbConStr = dbConnectionStringBuilder.ToString();
            
            _dbConnection = new NpgsqlConnection(dbConStr);
            _dbConnection.Open();
        }

        ~WikiStore()
        {
            _dbConnection.Close();
        }

        public async Task<Wiki> GetWikiByName(string wikiName, WikiUser wikiUser)
        {
            var cmd = new NpgsqlCommand(@"SELECT *
                                                   FROM wikis
                                                   WHERE name = @name
                                                   AND user_id = @user_id", _dbConnection);
            
            cmd.Parameters.AddWithValue("user_id", NpgsqlDbType.Uuid, wikiUser.Id);
            cmd.Parameters.AddWithValue("name", NpgsqlDbType.Text, wikiName);

            DbDataReader reader = await cmd.ExecuteReaderAsync();

            if (!reader.Read())
            {
                // no wiki found
                return null;
            }

            var wikiId = (Guid) reader["id"];
            var wikiBody = (string) reader["body"];

            var tagsFromReader = reader["tags"];
            var wikiTags = tagsFromReader is DBNull ? Enumerable.Empty<string>() : (string[]) tagsFromReader;

            // TODO: make sure wiki name is unique (set in db table)
            
            return new Wiki(wikiId, wikiName, wikiBody, wikiTags);
        }

        public async Task<IEnumerable<Wiki>> GetWikisWithTag(string tag, WikiUser wikiUser)
        {
            var cmd = new NpgsqlCommand(@"SELECT *
                                                   FROM wikis
                                                   WHERE @tag = ANY(tags)
                                                   AND user_id = @user_id", _dbConnection);
            
            cmd.Parameters.AddWithValue("user_id", NpgsqlDbType.Uuid, wikiUser.Id);
            cmd.Parameters.AddWithValue("tag", NpgsqlDbType.Text, tag);
            
            DbDataReader reader = await cmd.ExecuteReaderAsync();

            var wikis = new List<Wiki>();

            while (reader.Read())
            {
                var wikiId = (Guid) reader["id"];
                var wikiName = (string) reader["name"];
                var wikiBody = (string) reader["body"];
                
                var tagsFromReader = reader["tags"];
                var wikiTags = tagsFromReader is DBNull ? Enumerable.Empty<string>() : (string[]) tagsFromReader;

                // TODO: make sure wiki name is unique (set in db table)
                wikis.Add(new Wiki(wikiId, wikiName, wikiBody, wikiTags));
            }
            return wikis;
        }

        public async Task AddWiki(Wiki newWiki, WikiUser wikiUser)
        {
            if (string.IsNullOrWhiteSpace(newWiki.Name))
                return;

            NpgsqlCommand cmd = new NpgsqlCommand(@"INSERT INTO wikis(id, name, body, tags, user_id)
                                                         VALUES (@id, @name, @body, @tags, @user_id)", _dbConnection);
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
            NpgsqlCommand cmd = new NpgsqlCommand(@"DELETE FROM wikis
                                                             WHERE name=@name
                                                             AND user_id=@user_id", _dbConnection);
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
            NpgsqlCommand cmd = new NpgsqlCommand(@"SELECT name
                                                             FROM wikis
                                                             WHERE user_id=@user_id", _dbConnection);
            
            cmd.Parameters.AddWithValue("user_id", NpgsqlDbType.Uuid, wikiUser.Id);
            
            DbDataReader reader = await cmd.ExecuteReaderAsync();

            var wikiNames = new List<string>();
            while (reader.Read())
            {
                wikiNames.Add((string) reader["name"]);
            }

            return wikiNames;
        }

        public async Task UpdateWiki(Wiki updatedWiki, WikiUser wikiUser)
        {
            NpgsqlCommand cmd = new NpgsqlCommand(@"UPDATE wikis
                                                             SET body = @body, tags = @tags
                                                             WHERE name = @name
                                                             AND user_id=@user_id", _dbConnection);
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
            NpgsqlCommand cmd = new NpgsqlCommand(@"select array_agg(c) tags
                                                             FROM (
                                                                 SELECT unnest(tags)
                                                                 FROM wikis
                                                                 WHERE user_id=@user_id
                                                             ) AS dt(c)", _dbConnection);
            
            cmd.Parameters.AddWithValue("user_id", NpgsqlDbType.Uuid, wikiUser.Id);
            
            DbDataReader reader = await cmd.ExecuteReaderAsync();

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