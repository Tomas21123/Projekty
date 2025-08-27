using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlovníFotbal
{
    internal class Databaze
    {
        private string connectionString = "Server=db.dw239.endora.cz;User ID=uzivatelSlovniF;Password=SlovniF123@;Database=podsednikt_endor;SslMode=Preferred;";
        private MySqlConnection? conn;

        public Databaze()
        {
            conn = new MySqlConnection(connectionString);
        }

        public async Task<string?> NajdiSlovoZacinajiciNaAsync(char start)
        {
            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            const string colSql = @"
                SELECT COLUMN_NAME
                FROM information_schema.columns
                WHERE table_schema = DATABASE()
                  AND table_name = 'slovniFotbal'
                  AND DATA_TYPE IN ('char','varchar','text','tinytext','mediumtext','longtext');
            ";

            var candidateColumns = new List<string>();
            await using (var colCmd = new MySqlCommand(colSql, connection))
            await using (var reader = await colCmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    candidateColumns.Add(reader.GetString(0));
                }
            }

            if (candidateColumns.Count == 0)
            {
                return null;
            }

            string pattern = (start.ToString()).ToLower() + "%";

            foreach (var col in candidateColumns)
            {
                string sql = $"SELECT `{col}` FROM `slovniFotbal` WHERE LOWER(`{col}`) LIKE @pattern LIMIT 1;";

                await using var cmd = new MySqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@pattern", pattern);

                var result = await cmd.ExecuteScalarAsync();
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToString(result);
                }
            }

            return null;
        }

        public void Dispose()
        {
            conn?.Dispose();
            conn = null;
        }
    }
}

