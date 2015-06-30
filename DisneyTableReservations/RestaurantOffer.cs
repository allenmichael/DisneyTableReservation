using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisneyTableReservations
{
    public class RestaurantOffer
    {
        public String RestaurantId { get; set; }
        public DateTime ServiceDate { get; set; }
        public String OfferLink { get; set; }
        public String _restaurantName { get; private set; }
        
        public RestaurantOffer(String restaurantId, DateTime serviceDate, String offerLink)
        {
            RestaurantId = restaurantId;
            ServiceDate = serviceDate;
            OfferLink = offerLink;
            
            string connStr = "server=localhost;user=root;port=3306;password=password;database=disneyrestaurants";
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    Console.WriteLine("Connecting to MySQL...");
                    conn.Open();
                    String query = "SELECT restaurant_name FROM restaurants WHERE id='" + restaurantId + "'";
                    MySqlCommand cmd = new MySqlCommand();

                    cmd.CommandText = query;
                    cmd.Connection = conn;
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        _restaurantName = (String)rdr[0];
                    }
                    rdr.Close();
                    conn.Close();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
