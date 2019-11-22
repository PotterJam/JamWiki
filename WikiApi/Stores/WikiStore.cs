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
 
        private readonly NpgsqlConnection dbConnection;
        
        public WikiStore()
        {
            dbConnection = new NpgsqlConnection(postgresConStr);
            dbConnection.Open();
        }

        ~WikiStore()
        {
            dbConnection.Close();
        }

        public async Task<Wiki> GetWiki(Guid wikiId)
        {
            NpgsqlCommand cmd = new NpgsqlCommand(@"SELECT *
                                                             FROM wiki_page
                                                             WHERE id = @id", dbConnection);
            cmd.Parameters.AddWithValue("id", NpgsqlDbType.Uuid, wikiId);
            
            DbDataReader reader = await cmd.ExecuteReaderAsync();

            if (!reader.Read())
            {
                // no wiki found
                return null;
            }

            var wikiName = (string) reader["name"];

            var tagsFromReader = reader["tags"];
            var wikiTags = new string[] { };
            if (!(tagsFromReader is DBNull))
            {
                wikiTags = (string[]) tagsFromReader;
            }

            return new Wiki(wikiId, wikiName, wikiTags);
        }
    }
}