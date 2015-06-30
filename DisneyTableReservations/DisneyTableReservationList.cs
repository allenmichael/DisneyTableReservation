using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace DisneyTableReservations
{
    public class DisneyTableReservationList
    {

        private String _url = "https://api.wdpro.disney.go.com/availability-service/destinations/80007798/grouped-table-service-availability";
        private String _partySize;
        private String _mealPeriod;
        private String _searchDate;
        public List<RestaurantOffer> RestaurantIdsWithOffers { get; set; }
        
        
        public DisneyTableReservationList(AuthToken token, String partySize, String mealPeriod, String searchDate)
        {

            checkDateRange(checkDateFormat(searchDate));
            _searchDate = searchDate;
            _mealPeriod = mealPeriod;
            _partySize = partySize;

            String docPath = searchDate + "_" +  mealPeriod; 

            token = token.returnNewTokenWhenExpired();
            DisneyPostRequest reservationRequest = new DisneyPostRequest(_url, token, _partySize, _mealPeriod, _searchDate);
            this.parseRestaurantIdsFromServerResponse(reservationRequest.ResponseMessage, docPath);

        }

        private void parseRestaurantIdsFromServerResponse(String response, String docPath)
        {
            this.RestaurantIdsWithOffers = new List<RestaurantOffer>();
            
            try
            {
                JObject responseToJson = JObject.Parse(response);
                JContainer restaurants = (JContainer)responseToJson.SelectToken("restaurants");
                JEnumerable<JContainer> allRestaurantIds = restaurants.Children<JContainer>();
                string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                using (StreamWriter outfile = new StreamWriter(mydocpath + @"\DisneyReservations" + docPath + ".txt"))
                {


                    foreach (var offer in allRestaurantIds)
                    {
                        foreach (var off in offer)
                        {
                            var available = off.SelectToken("offers");
                            if (available != null)
                            {
                                foreach (var service in available)
                                {
                                    String restaurantId = (String)service.SelectToken("links").SelectToken("restaurant").SelectToken("href");
                                    restaurantId = restaurantId.Substring(restaurantId.IndexOf("restaurants/")).Split('/')[1];
                                    DateTime serviceDate = (DateTime)service.SelectToken("serviceDatetime");
                                    serviceDate = serviceDate.AddHours(1);
                                    String offerLink = (String)service.SelectToken("links").SelectToken("self").SelectToken("href");

                                    //Use this constructor for REST API call to resolve restaurant name
                                    RestaurantOffer uniqueOffer = new RestaurantOffer(restaurantId, serviceDate, offerLink, "http://amburger.com/disneyrestaurants/?id=");
                                    
                                    //Use this constructor for local database call to resolve restaurant name
                                    //RestaurantOffer uniqueOffer = new RestaurantOffer(restaurantId, serviceDate, offerLink);

                                    RestaurantIdsWithOffers.Add(uniqueOffer);

                                    outfile.WriteLine("*****");
                                    outfile.WriteLine(uniqueOffer._restaurantName);
                                    outfile.WriteLine(uniqueOffer.RestaurantId);
                                    outfile.WriteLine(uniqueOffer.ServiceDate.ToString());
                                    outfile.WriteLine("https://disneyworld.disney.go.com/dining-reservation/book-table-service/?offerId[]=" + uniqueOffer.OfferLink);
                                    outfile.WriteLine("*****");
                                    outfile.WriteLine("");

                                }

                            }
                        }
                    }
                }

                
                    
            }
            catch(JsonReaderException err)
            {
                Console.WriteLine(err.Message);
            }
            

            
            
        }


        private static String checkDateFormat(String searchDate)
        {
            Regex datePattern = new Regex(@"[0-9]{4}-[0-9]{2}-[0-9]{2}");
            while(!datePattern.IsMatch(searchDate))
            {
                Console.WriteLine("Please enter the date in this format: yyyy/mm/dd");
                Console.Write("Please enter the search date for your reservations: ");
                try
                {
                    searchDate = Console.ReadLine();
                }
                catch
                {
                    Console.Write("Please enter a valid search date for your reservations: ");
                    searchDate = Console.ReadLine();
                }
                
            }

            return searchDate;
        }

        private static String checkDateRange(String searchDate)
        {
            DateTime enteredDate = Convert.ToDateTime(searchDate);
            DateTime compareDate = DateTime.Now;
            DateTime futureDate = compareDate.AddYears(1);
            while(!((DateTime.Compare(enteredDate, compareDate) >= 0 && DateTime.Compare(enteredDate, futureDate) < 0)))
            {
                Console.WriteLine("Please enter a date before {0} or after {1} ", futureDate.ToString("yyyy-MM-dd"), compareDate.ToString("yyyy-MM-dd"));
                Console.Write("Please enter the search date for your reservations: ");
                searchDate = Console.ReadLine();
                try
                {
                    enteredDate = Convert.ToDateTime(searchDate);
                }
                catch
                {
                    Console.Write("Please enter a valid search date for your reservations: ");
                    searchDate = Console.ReadLine();
                }
            }
            
            return searchDate;
        }
    }
}
