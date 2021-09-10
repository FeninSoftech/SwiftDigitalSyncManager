using SwiftDigitalSyncManager.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SwiftDigitalSyncManager.OAuth
{
    public class OAuthParameters
    {
        private string Nonce { get; set; }
        public string TimeStamp { get; set; }
        public Apitoken AuthToken { get; set; }

        public OAuthParameters()
        {
            TimeStamp = Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds).ToString(CultureInfo.InvariantCulture);
            Nonce = Guid.NewGuid().ToString();
        }

        public string CreateSignature(string url, string method, Dictionary<string, string> parameters)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(method.ToUpper() + "&");
            stringBuilder.Append(Uri.EscapeDataString(url.ToLower()));
            stringBuilder.Append("&");

            //the key value pairs have to be sorted by encoded key
            var dictionary = new SortedDictionary<string, string>
                                 {
                                     {"oauth_consumer_key", AuthToken.consumer_key},
                                     {"oauth_nonce", Nonce},
                                     {"oauth_signature_method", "HMAC-SHA1"},
                                     {"oauth_timestamp", TimeStamp},
                                     {"oauth_token", AuthToken.oauth_token},
                                     {"oauth_version", "1.0"}
                                 };

            foreach (KeyValuePair<string, string> param in parameters)
                dictionary.Add(param.Key, param.Value);

            foreach (var keyValuePair in dictionary)
            {
                stringBuilder.Append(Uri.EscapeDataString(string.Format("{0}={1}&", keyValuePair.Key, keyValuePair.Value)));
            }
            string signatureBaseString = stringBuilder.ToString().Substring(0, stringBuilder.Length - 3);

            //generation the signature key the hash will use
            string signatureKey = Uri.EscapeDataString(AuthToken.ClientSecret) + "&" + AuthToken.oauth_token_secret;

            var hmacsha1 = new HMACSHA1(new ASCIIEncoding().GetBytes(signatureKey));

            //hash the values
            string signatureString = Convert.ToBase64String(hmacsha1.ComputeHash(new ASCIIEncoding().GetBytes(signatureBaseString)));

            return signatureString;
        }

        public string CreateAuthorizationHeaderParameter(string signature)
        {
            string authorizationHeaderParams = String.Empty;
            authorizationHeaderParams += "OAuth ";
            authorizationHeaderParams += "oauth_consumer_key=" + "\"" + Uri.EscapeDataString(AuthToken.consumer_key) + "\",";
            authorizationHeaderParams += "oauth_token=" + "\"" + Uri.EscapeDataString(AuthToken.oauth_token) + "\",";
            authorizationHeaderParams += "oauth_signature_method=" + "\"" + Uri.EscapeDataString("HMAC-SHA1") + "\",";
            authorizationHeaderParams += "oauth_timestamp=" + "\"" + Uri.EscapeDataString(TimeStamp) + "\",";
            authorizationHeaderParams += "oauth_nonce=" + "\"" + Uri.EscapeDataString(Nonce) + "\",";
            authorizationHeaderParams += "oauth_version=" + "\"" + Uri.EscapeDataString("1.0") + "\",";
            authorizationHeaderParams += "oauth_signature=" + "\"" + Uri.EscapeDataString(signature) + "\"";

            return authorizationHeaderParams;
        }
    }
}
