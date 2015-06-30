using MySql.Data;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;



namespace DisneyTableReservations
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Starting...");
            String mealPeriod;
            String searchDate;

            Console.Write("Enter your meal period (breakfast, lunch, or dinner): ");
            mealPeriod = Console.ReadLine();
            Console.Write("Enter your search date: yyyy-MM-dd: ");
            searchDate = Console.ReadLine();
            AuthToken token = new AuthToken();
            DisneyTableReservationList reservationList = new DisneyTableReservationList(token, "2", mealPeriod, searchDate);

            Console.WriteLine("Press Enter to exit.");
            Console.Read();
                        
        }
    }
}
