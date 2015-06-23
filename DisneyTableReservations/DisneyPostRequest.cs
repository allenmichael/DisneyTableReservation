using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Cache;

namespace DisneyTableReservations
{
    public class DisneyPostRequest : IDisneyRequest, IDisneyAuthorizedRequest, IDisneyReservationRequest
    {

        public String ResponseMessage { get; private set; }
        private HttpWebResponse _response;
        private String _reservationUrl;
        
        public DisneyPostRequest(String url)
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
            catch(NullReferenceException err)
            {
                Console.WriteLine(err.Message);
            }
        }

        public DisneyPostRequest(String url, AuthToken token)
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

        public DisneyPostRequest(String url, AuthToken token, String partySize, String mealPeriod, String searchDate)
        {
            String parameters = "grant_type=assertion&assertion_type=public&client_id=WDPRO-MOBILE.CLIENT-PROD&partySize=" + partySize + "&mealPeriod=" + mealPeriod + "&searchDate=" + searchDate;

            IDisneyReservationRequest disneyRequest = this;
            HttpWebRequest request = disneyRequest.setHeadersWithAuthorization(url, token);
            HttpWebResponse response = disneyRequest.makeReservationRequest(request, parameters);
            try
            {
                _response = response;
                String responseMessage = disneyRequest.returnResponse(response);
                ResponseMessage = responseMessage;
                WebHeaderCollection responseHeaders = response.Headers;
                _reservationUrl = responseHeaders.Get("Location");

            }
            catch (NullReferenceException err)
            {
                Console.WriteLine(err.Message);
                
            }

            DisneyGetRequest reservationRequest = new DisneyGetRequest(_reservationUrl, token);
            ResponseMessage = reservationRequest.ResponseMessage;

        }

        HttpWebRequest IDisneyRequest.setHeaders(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            return request;
        }

        HttpWebRequest IDisneyAuthorizedRequest.setHeadersWithAuthorization(string url, AuthToken token)
        {
            token = token.returnNewTokenWhenExpired();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add("Authorization", "Bearer" + " " + token.Token);
            request.ContentType = "application/x-www-form-urlencoded";
            
            return request;
        }

        HttpWebResponse IDisneyRequest.makeRequest(HttpWebRequest request)
        {
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            

            string parameters = "grant_type=assertion&assertion_type=public&client_id=WDPRO-MOBILE.CLIENT-PROD";
            byte[] postData = Encoding.UTF8.GetBytes(parameters);
            request.ContentLength = postData.Length;
            
            try
            {
                
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(postData, 0, postData.Length);
                requestStream.Close();
                try
                {
                    
                    WebResponse response = request.GetResponse();
                    return ((HttpWebResponse)response);
                }
                catch (WebException err)
                {
                    Console.WriteLine(err.Message);                  

                }
            }
            catch(WebException err)
            {
                Console.WriteLine(err.Message + "\nCouldn't access this URL. Please check your URL and try again later.");
            }
            
            return null;


        }

        HttpWebResponse IDisneyReservationRequest.makeReservationRequest(HttpWebRequest request, String parameters)
        {
            
            request.Method = "POST";
            request.AllowAutoRedirect = false;
            
            byte[] postData = Encoding.UTF8.GetBytes(parameters);
            
            request.ContentLength = postData.Length;
            
            
            try
            {
                
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(postData, 0, postData.Length);
                requestStream.Close();
                try
                {
                    WebResponse response = request.GetResponse();
                    
                    return ((HttpWebResponse)response);
                    
                }
                catch (WebException err)
                {
                    Console.WriteLine(err.Message);
                    WebResponse errResponse = err.Response;
                    Stream responseStream = errResponse.GetResponseStream();
                    StreamReader reader = new StreamReader(responseStream);
                    string responseFromServer = reader.ReadToEnd();
                    reader.Close();
                    responseStream.Close();
                    errResponse.Close();
                    Console.WriteLine(responseFromServer);
                }
            }
            catch (WebException err)
            {
                Console.WriteLine(err.Message + "\nCouldn't access this URL. Please check your URL and try again later.");
            }

            return null;
            
        }

        String IDisneyRequest.returnResponse(HttpWebResponse response) 
        {
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);

            string responseFromServer = reader.ReadToEnd();
            
            reader.Close();
            responseStream.Close();
            response.Close();

            return responseFromServer;
        }






    }
}
