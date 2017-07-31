using LagerApp.DTOs;
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

        //Speichern des Artikel zur Box: A15000 zu BOX100
        //Falls A15000 schon vorhanden, dann update von BOX
        public int saveOrUpdateToBox(LagerBoxDTO dto)
        {
            int rowsAffected = 0;
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                if (dto.ArtikelId != null && dto.LagerBox != null)
                {
                    MySqlCommand cmd = new MySqlCommand("INSERT INTO artikel (artikel_id,lagerbox_id,lagerplatz_id) " +
                        "VALUES (@artikel,@box,@platz) ON DUPLICATE KEY UPDATE lagerbox_id=@box", conn);
                    cmd.Parameters.AddWithValue("@artikel", dto.ArtikelId);
                    cmd.Parameters.AddWithValue("@box", dto.LagerBox);
                    cmd.Parameters.AddWithValue("@platz", null);
                    rowsAffected = cmd.ExecuteNonQuery();
                }
                conn.Close();

            }
            return rowsAffected;
        }

        //Speichern des Artikel zum LagerPlatz: A10000 zu A01-02-01
        public int saveOrUpdateArtikelToPlatz(ArtikelLagerPlatzDTO dto)
        {
            int rowsAffected = 0;
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                if (dto.LagerPlatz != null && dto.LagerPlatz != null)
                {
                    MySqlCommand cmd = new MySqlCommand("INSERT INTO artikel (artikel_id,lagerbox_id,lagerplatz_id) " +
                         "VALUES (@artikel,@box,@platz) ON DUPLICATE KEY UPDATE lagerplatz_id=@platz", conn);
                    cmd.Parameters.AddWithValue("@artikel", dto.ArtikelId);
                    cmd.Parameters.AddWithValue("@platz", dto.LagerPlatz);
                    cmd.Parameters.AddWithValue("@box", null);
                   rowsAffected = cmd.ExecuteNonQuery();
                }
                conn.Close();

            }
            return rowsAffected;
        }

        //Speichern der Box zum LagerPlatz: BOX100 zu A01-02-01
        public int saveOrUpdateBoxToPlatz(LagerPlatzDTO dto)
        {
            int rowsAffected = 0;
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                if (dto.LagerPlatz != null && dto.LagerBox != null)
                {
                    MySqlCommand cmd = new MySqlCommand("UPDATE lagerbox SET lagerplatz_box_id=@lager " +
                        "WHERE lagerbox_id=@box", conn);
                    cmd.Parameters.AddWithValue("@lager", dto.LagerPlatz);
                    cmd.Parameters.AddWithValue("@box", dto.LagerBox);
                    rowsAffected = cmd.ExecuteNonQuery();
                }
                conn.Close();

            }
            return rowsAffected;
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
                    "ON a.lagerbox_id = b.lagerbox_id) LEFT OUTER JOIN lagerplatz p ON b.lagerplatz_box_id = p.lagerplatz_id", conn);

                String lagerplatz_id_ohne_box;
                String lagerplatz_id;
                String lagerbox_id;

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {   //LagerPlatz ohne Box
                        if (reader.IsDBNull(2))
                        {
                            lagerplatz_id_ohne_box = "";
                        }
                        else
                        {
                            lagerplatz_id_ohne_box = reader.GetString("lagerplatz_id");
                        }
                        //LagerPlatz mit Box
                        if (reader.IsDBNull(4))
                        {
                            lagerplatz_id = "";
                        }
                        else
                        {
                            lagerplatz_id = reader.GetString("lagerplatz_box_id");
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
                            LagerPlatzOhneBox = lagerplatz_id_ohne_box,
                            LagerBox = lagerbox_id
                        });
                    }
                }
            }

            return list;
        }

    }
}
