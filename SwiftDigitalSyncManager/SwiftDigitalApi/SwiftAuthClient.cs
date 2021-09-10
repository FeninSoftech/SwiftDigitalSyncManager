using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwiftDigitalSyncManager.Entities;
using SwiftDigitalSyncManager.Helpers;
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SwiftDigitalSyncManager.SwiftDigitalApi
{
    public class SwiftAuthClient
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static AuthResponse GetAccessToken()
        {
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(AppConfiguration.APIBaseUrl.TrimEnd('/') + "?request=auth/gettoken");
            objRequest.Method = WebRequestMethods.Http.Post;
            

            try
            {
                var authClient = new { apiCallSource = AppConfiguration.APICallSource, consumer_key = AppConfiguration.ConsumerKey, user_id = AppConfiguration.UserId };

                using (var requestStream = objRequest.GetRequestStream())
                {
                    var authJson = JsonConvert.SerializeObject(authClient);

                    var bytes = Encoding.ASCII.GetBytes(authJson);
                    requestStream.Write(bytes, 0, bytes.Length);
                }

                HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
                if (objResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream responseStream = objResponse.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            string json = reader.ReadToEnd();
                            if (json.Contains("error"))
                            {
                                log.Error("Error in method GetAccessToken: " + json);
                            }

                            AuthResponse authToken = JsonConvert.DeserializeObject<AuthResponse>(json);

                            reader.Close();
                            responseStream.Close();

                            return authToken;
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                log.Error(ex);

                return null;
            }
        }
    }
}
