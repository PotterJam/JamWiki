using System;
using Npgsql;
using NpgsqlTypes;

namespace WikiApi
{
    class Program
    {
        static void Main(string[] args)
        {
            string postgresConStr = "Server=localhost;Port=5433;UserId=postgres;Password=darkanima;Database=wikiapi;";
            NpgsqlConnection dbConnection = new NpgsqlConnection(postgresConStr);
            NpgsqlCommand cmd = new NpgsqlCommand(@"INSERT INTO wiki_page (id, name)
                                                             VALUES (@id, 'first_wiki')", dbConnection);
            cmd.Parameters.AddWithValue("id", NpgsqlDbType.Uuid, Guid.NewGuid());
            dbConnection.Open();
            NpgsqlDataReader dr = cmd.ExecuteReader();
            var insertResult = dr.Read();
            dbConnection.Close();
            Console.WriteLine("Hello World!");
        }
    }
}