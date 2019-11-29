﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;

namespace WikiApi
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

        public async Task<Wiki> GetWikiByName(string wikiName)
        {
            var cmd = new NpgsqlCommand(@"SELECT *
                                                             FROM wikis
                                                             WHERE name = @name", _dbConnection);
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
            var wikiTags = new string[] { };
            if (!(tagsFromReader is DBNull))
            {
                wikiTags = (string[]) tagsFromReader;
            }

            // TODO: make sure wiki name is unique (set in db table)
            
            return new Wiki(wikiId, wikiName, wikiBody, wikiTags);
        }

        public async Task AddWiki(Wiki newWiki)
        {
            NpgsqlCommand cmd = new NpgsqlCommand(@"INSERT INTO wikis(id, name, body, tags)
                                                             VALUES (@id, @name, @body, @tags)", _dbConnection);
            cmd.Parameters.AddWithValue("id", NpgsqlDbType.Uuid, newWiki.Id);
            cmd.Parameters.AddWithValue("name", NpgsqlDbType.Text, newWiki.Name);
            cmd.Parameters.AddWithValue("body", NpgsqlDbType.Text, newWiki.Body);
            cmd.Parameters.AddWithValue("tags", NpgsqlDbType.Array | NpgsqlDbType.Text, newWiki.Tags);

            var rowsChanged = await cmd.ExecuteNonQueryAsync();
            if (rowsChanged != 1)
            {
                throw new NpgsqlException("Added more than one wiki, something has gone seriously wrong. ");
            }
        }

        public async Task DeleteWikiByName(string name)
        {
            NpgsqlCommand cmd = new NpgsqlCommand(@"DELETE FROM wikis
                                                             WHERE name=@name", _dbConnection);
            cmd.Parameters.AddWithValue("name", NpgsqlDbType.Text, name);

            var rowsChanged = await cmd.ExecuteNonQueryAsync();
            if (rowsChanged != 1)
            {
                throw new NpgsqlException("Deleted more than one wiki, something has gone seriously wrong. ");
            };
        }

        public async Task<IEnumerable<string>> GetWikiNames()
        {
            NpgsqlCommand cmd = new NpgsqlCommand(@"SELECT name
                                                             FROM wikis", _dbConnection);
            
            DbDataReader reader = await cmd.ExecuteReaderAsync();

            var wikiNames = new List<string>();
            while (reader.Read())
            {
                wikiNames.Add((string) reader["name"]);
            }

            return wikiNames;
        }

        public async Task UpdateWiki(Wiki updatedWiki)
        {
            NpgsqlCommand cmd = new NpgsqlCommand(@"UPDATE wikis SET body = @body, tags = @tags WHERE name = @name", _dbConnection);
            cmd.Parameters.AddWithValue("name", NpgsqlDbType.Text, updatedWiki.Name);
            cmd.Parameters.AddWithValue("body", NpgsqlDbType.Text, updatedWiki.Body);
            cmd.Parameters.AddWithValue("tags", NpgsqlDbType.Array | NpgsqlDbType.Text, updatedWiki.Tags);

            var rowsChanged = await cmd.ExecuteNonQueryAsync();
            if (rowsChanged != 1)
            {
                throw new NpgsqlException("Added more than one wiki, something has gone seriously wrong. ");
            }
        }
    }
}