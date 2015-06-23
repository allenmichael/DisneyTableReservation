using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisneyTableReservations
{
    public class DisneyRestaurant
    {
        public int Id { get;  set; }
        public String RestaurantName { get;  set; }
        public String RestaurantUrl { get;  set; }
        public String RestaurantType { get;  set; }
        public String Location { get;  set; }
        public String PriceRange { get; set; }

        public DisneyRestaurant()
        {
            Id = 0;
        }
    }
}
