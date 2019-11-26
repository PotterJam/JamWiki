using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;

namespace WikiApi
{
    public class WikiStore : IWikiStore
    {
        private readonly string postgresConStr = "Server=localhost;Port=5433;UserId=postgres;Password=darkanima;Database=wikiapi;";
 
        private readonly NpgsqlConnection _dbConnection;
        
        public WikiStore()
        {
            _dbConnection = new NpgsqlConnection(postgresConStr);
            _dbConnection.Open();
        }

        ~WikiStore()
        {
            _dbConnection.Close();
        }

        public async Task<Wiki> GetWikiByName(string wikiName)
        {
            NpgsqlCommand cmd = new NpgsqlCommand(@"SELECT *
                                                             FROM wiki_page
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
            NpgsqlCommand cmd = new NpgsqlCommand(@"INSERT INTO wiki_page(id, name, body, tags)
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
            NpgsqlCommand cmd = new NpgsqlCommand(@"DELETE FROM wiki_page
                                                             WHERE name=@name", _dbConnection);
            cmd.Parameters.AddWithValue("name", NpgsqlDbType.Text, name);

            var rowsChanged = await cmd.ExecuteNonQueryAsync();
            if (rowsChanged != 1)
            {
                throw new NpgsqlException("Deleted more than one wiki, something has gone seriously wrong. ");
            };
        }
    }
}