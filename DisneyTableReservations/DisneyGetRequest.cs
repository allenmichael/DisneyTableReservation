using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DisneyTableReservations
{
    public class DisneyGetRequest : IDisneyRequest, IDisneyAuthorizedRequest
    {
        public String ResponseMessage { get; private set; }
        private HttpWebResponse _response;

        public DisneyGetRequest(String url)
        {
            IDisneyRequest disneyRequest = this;
            HttpWebRequest request = disneyRequest.setHeaders(url);
            HttpWebResponse response = disneyRequest.makeRequest(request);
            try
            {
                _response = response;
                String responseMessage = disneyRequest.returnResponse(response);
                ResponseMessage = responseMessage;
            }
            catch (NullReferenceException err)
            {
                Console.WriteLine(err.Message);
            }
        }
        
        public DisneyGetRequest(String url, AuthToken token)
        {
            IDisneyAuthorizedRequest disneyRequest = this;
            HttpWebRequest request = disneyRequest.setHeadersWithAuthorization(url, token);
            HttpWebResponse response = disneyRequest.makeRequest(request);
            try
            {
                _response = response;
                String responseMessage = disneyRequest.returnResponse(response);
                ResponseMessage = responseMessage;
            }
            catch (NullReferenceException err)
            {
                Console.WriteLine(err.Message);
            }
        }

        HttpWebRequest IDisneyRequest.setHeaders(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            return request;
        }

        HttpWebResponse IDisneyRequest.makeRequest(HttpWebRequest request)
        {
            request.Method = "GET";

            try
            {
                WebResponse response = request.GetResponse();
                return ((HttpWebResponse)response);
            }
            catch (WebException err)
            {
                Console.WriteLine(err.Message);
            }

            return null;
        }

        string IDisneyRequest.returnResponse(HttpWebResponse response)
        {
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);

            string responseFromServer = reader.ReadToEnd();

            reader.Close();
            responseStream.Close();
            response.Close();

            return responseFromServer;
        }

        HttpWebRequest IDisneyAuthorizedRequest.setHeadersWithAuthorization(string url, AuthToken token)
        {
            token = token.returnNewTokenWhenExpired();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add("Authorization", "Bearer " + token.Token);
            request.Accept = "application/json";
            request.Headers.Add("X-Conversation-Id", "WDPRO-MOBILE.CLIENT-PROD");
            
            return request;
        }
    }
}
