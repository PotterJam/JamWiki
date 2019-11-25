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
    }
}