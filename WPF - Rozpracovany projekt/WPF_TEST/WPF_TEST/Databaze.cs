using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using MySqlConnector;
using WPF_TEST.Knihovna;

namespace WPF_TEST
{
    internal class Databaze : IDisposable
    {
        private string connectionString = "Server=db.dw239.endora.cz;User ID=podsednikt_endor;Password=zmT3BWkn;Database=podsednikt_endor;SslMode=Preferred;";
        private MySqlConnection? conn;

        public Databaze() 
        {
            conn = new MySqlConnection(connectionString);
        }



        public async Task<bool> kontrolaLogin(string e, string h )
        {
            const string sql = "SELECT COUNT(*) FROM uzivatele WHERE email = @Email AND heslo = @Password;";
            await conn.OpenAsync();
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Email", e);
            cmd.Parameters.AddWithValue("@Password", h);

            var result = await cmd.ExecuteScalarAsync();
            int count = Convert.ToInt32(result);
            return count >= 1;
        }



        public async Task<bool> kontrolaRegistrace(string e)
        {
            const string sql = "SELECT COUNT(*) FROM uzivatele WHERE email = @Email;";
            await conn.OpenAsync();
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Email", e);

            var result = await cmd.ExecuteScalarAsync();
            int count = Convert.ToInt32(result);
            return count == 0;
        }



        public async Task registraceAsync(string e, string h)
        {
            const string sql = "INSERT INTO `uzivatele`(`email`, `heslo`) VALUES (@Email, @Heslo);";

            await using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();

            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Email", e);
            cmd.Parameters.AddWithValue("@Heslo", h);

            await cmd.ExecuteNonQueryAsync();
        }



        // doporučený návratový typ pro async metoda vracející kolekci
        public async Task<List<Kniha>> NavratSeznamKnihAsync()
        {
            var result = new List<Kniha>();

            const string sql = "SELECT id, autor, nazev, zanr, litDruh FROM seznamKnih;";

            await using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();

            await using var cmd = new MySqlCommand(sql, conn);

            await using var reader = await cmd.ExecuteReaderAsync();
            // získáme indexy sloupců jednou pro rychlost a čitelnost
            int ordId = reader.GetOrdinal("id");
            int ordAutor = reader.GetOrdinal("autor");
            int ordNazev = reader.GetOrdinal("nazev");
            int ordZanr = reader.GetOrdinal("zanr");
            int ordLitDruh = reader.GetOrdinal("litDruh");

            while (await reader.ReadAsync())
            {
                var book = new Kniha
                {
                    Id = reader.IsDBNull(ordId) ? 0 : reader.GetInt32(ordId),
                    Autor = reader.IsDBNull(ordAutor) ? string.Empty : reader.GetString(ordAutor),
                    Nazev = reader.IsDBNull(ordNazev) ? string.Empty : reader.GetString(ordNazev),
                    Zanr = reader.IsDBNull(ordZanr) ? string.Empty : reader.GetString(ordZanr),
                    LitDruh = reader.IsDBNull(ordLitDruh) ? string.Empty : reader.GetString(ordLitDruh)
                };
                result.Add(book);
            }

            return result;
        }




        public void Dispose()
        {
            conn?.Dispose();
        }
    }
}
