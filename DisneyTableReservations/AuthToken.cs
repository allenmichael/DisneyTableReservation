using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DisneyTableReservations
{
    public class AuthToken
    {
        private Dictionary<String, String> _authJsonResponse;
        private String _tokenUrl = "https://authorization.go.com/token";
        private DateTime _createdAt;
        private double _expirationSeconds;
        private DateTime _expiresAt;
        public String Token { get; private set; }

        public AuthToken()
        {
            DisneyPostRequest tokenRequest = new DisneyPostRequest(this._tokenUrl);
            try
            {
                _authJsonResponse = this.parseServerResponse(tokenRequest.ResponseMessage);
                this.Token = _authJsonResponse["access_token"];
                this._createdAt = DateTime.Now;
                this._expirationSeconds = Convert.ToDouble(_authJsonResponse["expires_in"]);
                this._expiresAt = _createdAt.AddSeconds(_expirationSeconds);
            }
            catch(NullReferenceException err)
            {
                Console.WriteLine(err.Message + "\nCouldn't get access token, try again later.");
            }
            catch(OverflowException overflow)
            {
                Console.WriteLine(overflow.Message);
            }

        }
        
        public AuthToken returnNewTokenWhenExpired()
        {
            if(_createdAt > _expiresAt)
            {
                return new AuthToken();
            }
            else
            {
                return this;
            }
        }

        private Dictionary<String, String> parseServerResponse(String response)
        {
            Dictionary<String, String> responseAsJSON = JsonConvert.DeserializeObject<Dictionary<String, String>>(response);
            return responseAsJSON;
        }

    }
}
