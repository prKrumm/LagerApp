using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagerApp.Model
{
    public class LagerContext
    {
        public string ConnectionString { get; set; }

        public LagerContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public List<Artikel> GetAllArtikel()
        {
            List<Artikel> list = new List<Artikel>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                //MySqlCommand cmd = new MySqlCommand("SELECT * FROM artikel a,lagerbox b,lagerplatz p " +
                //    "where a.lagerbox_id = b.lagerbox_id AND b.lagerplatz_id = p.lagerplatz_id", conn);

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM (artikel a LEFT JOIN lagerbox b " +
                    "ON a.lagerbox_id = b.lagerbox_id) LEFT OUTER JOIN lagerplatz p ON b.lagerplatz_id = p.lagerplatz_id", conn);

                String lagerplatz_id;
                String lagerbox_id;

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.IsDBNull(3))
                        {
                            lagerplatz_id = "";
                        }
                        else
                        {
                            lagerplatz_id = reader.GetString("lagerplatz_id");
                        }
                        if (reader.IsDBNull(1))
                        {
                            lagerbox_id = "";
                        }
                        else
                        {
                            lagerbox_id = reader.GetString("lagerbox_id");
                        }



                        list.Add(new Artikel()
                        {
                            ArtikelId = reader.GetString("artikel_id"),                           
                            LagerPlatz = lagerplatz_id,
                            LagerBox = lagerbox_id
                        });
                    }
                }
            }

            return list;
        }

    }
}
