using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DisneyTableReservations
{
    interface IDisneyReservationRequest : IDisneyRequest, IDisneyAuthorizedRequest
    {

        HttpWebResponse makeReservationRequest(HttpWebRequest request, String parameters);

    }
}
