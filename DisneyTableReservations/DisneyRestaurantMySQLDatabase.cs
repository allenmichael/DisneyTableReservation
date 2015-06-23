using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisneyTableReservations
{
    public class DisneyRestaurantMySQLDatabase
    {
        public static void createMySQLDB()
        {
            string connStr = "server=localhost;user=root;port=3306;password=password;";
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();

                String query = "CREATE DATABASE IF NOT EXISTS disneyrestaurants;";
                MySqlCommand cmd = new MySqlCommand();

                cmd.CommandText = query;
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();

                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void createRestaurantTable()
        {
            string connStr = "server=localhost;user=root;port=3306;password=password;database=disneyrestaurants";
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();

                String query = "CREATE TABLE IF NOT EXISTS restaurants(id INT NOT NULL, restaurant_name VARCHAR(255) NOT NULL, restaurant_url VARCHAR(255) NOT NULL, restaurant_type VARCHAR(255), location VARCHAR(255), price_range VARCHAR(10), PRIMARY KEY (id));";
                MySqlCommand cmd = new MySqlCommand();

                cmd.CommandText = query;
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();

                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void addRestaurantToMySQLDB(DisneyRestaurant restaurant)
        {
            string connStr = "server=localhost;user=root;port=3306;password=password;database=disneyrestaurants";
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();

                String query = "INSERT INTO restaurants (id, restaurant_name, restaurant_url, restaurant_type, location, price_range) VALUES (\"" + restaurant.Id + "\", \"" + restaurant.RestaurantName + "\", \"" + restaurant.RestaurantUrl + "\", \"" + restaurant.RestaurantType + "\",\"" + restaurant.Location + "\", \"" + restaurant.PriceRange + "\");";
                MySqlCommand cmd = new MySqlCommand();

                cmd.CommandText = query;
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();

                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}


