using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisneyTableReservations
{
    public class FullRestaurantList
    {
        public List<String> _restaurantUrls { get; private set; }
        public Int32 _totalNumberOfRestaurants { get; private set; }
        public Dictionary<String, String> _restaurantNameAndId { get; private set; }
        private String _fullRestaurantListUrl = "https://api.wdpro.disney.go.com/global-pool-override-A/facility-service/restaurants/";
        public List<DisneyRestaurant> DisneyRestaurants { get; set; }

        public FullRestaurantList(AuthToken token)
        {
            populateRestaurantUrls(token);
            
        }

        public void populateDisneyRestaurants(AuthToken token)
        {
            this.DisneyRestaurants = new List<DisneyRestaurant>();

            foreach (String url in this._restaurantUrls)
            {

                DisneyRestaurant restaurant = new DisneyRestaurant();
                
                restaurant.RestaurantUrl = url;

                DisneyGetRequest restaurantRequest = new DisneyGetRequest(url, token);
                
                try
                {
                    //Location
                    JObject response = JObject.Parse(restaurantRequest.ResponseMessage);
                    
                    JObject links = (JObject)response.SelectToken("links");
                    JObject ancestorPark = (JObject)links.SelectToken("ancestorThemePark") ?? (JObject)links.SelectToken("ancestorEntertainmentVenue") ?? (JObject)links.SelectToken("ancestorResort");
                    restaurant.Location = (String)ancestorPark.SelectToken("title");
                    Console.WriteLine(restaurant.Location);

                    //ID
                    String id = (String)response.SelectToken("id");
                    String[] words = id.Split(';');
                    restaurant.Id = Convert.ToInt32(words[0]);
                    Console.WriteLine(restaurant.Id);

                    //Name
                    restaurant.RestaurantName = (String)response.SelectToken("name");
                    Console.WriteLine(restaurant.RestaurantName);

                    //Price Range
                    JObject facets = (JObject)response.SelectToken("facets");
                    JArray priceRange = (JArray)facets.SelectToken("priceRange");
                    restaurant.PriceRange = (String)priceRange[0].SelectToken("urlFriendlyId");
                    Console.WriteLine(restaurant.PriceRange);


                    // Type of restaurant -- Table or Quick
                    JArray service = (JArray)facets.SelectToken("tableService");
                    String diningType = null;
                    if (service != null)
                    {
                        JArray tableDiningMealTimesArray = (JArray)facets.SelectToken("dining");

                        diningType = "Table Service";
                    }
                    else
                    {
                        service = (JArray)facets.SelectToken("quickService");
                        if (service != null)
                        {
                            diningType = "Quick Service";
                        }
                    }
                    restaurant.RestaurantType = diningType;
                    Console.WriteLine(restaurant.RestaurantType);
                }
                catch(NullReferenceException err)
                {
                    Console.WriteLine(err.Message);
                }
                if (restaurant.Id != 0)
                {
                    this.DisneyRestaurants.Add(restaurant);
                    Console.WriteLine("Added restaurant!");
                }
               
            }

        }

        private void populateRestaurantUrls(AuthToken token)
        {
            List<String> urls = new List<String>();
            DisneyGetRequest request = new DisneyGetRequest(_fullRestaurantListUrl, token);
            JObject response = JObject.Parse(request.ResponseMessage);

            String total = (String)response.SelectToken("total");
            _totalNumberOfRestaurants = Convert.ToInt32(total);

            JArray entries = (JArray)response.SelectToken("entries");
            foreach(var entry in entries)
            {
                JObject links = (JObject)entry.SelectToken("links");
                JObject self = (JObject)links.SelectToken("self");
                string url = (String)self.SelectToken("href");
                urls.Add(url);
            }

            _restaurantUrls = urls;
        }

 

    }
}
