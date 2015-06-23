using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DisneyTableReservations
{
    public interface IDisneyRequest
    {
        HttpWebRequest setHeaders(string url);

        HttpWebResponse makeRequest(HttpWebRequest request);

        String returnResponse(HttpWebResponse response);
    }
}
