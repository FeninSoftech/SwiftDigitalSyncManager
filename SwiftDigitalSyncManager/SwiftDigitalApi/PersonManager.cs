using Newtonsoft.Json;
using SwiftDigitalSyncManager.Entities;
using SwiftDigitalSyncManager.Helpers;
using SwiftDigitalSyncManager.OAuth;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SwiftDigitalSyncManager.SwiftDigitalApi
{
    public class PersonManager
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static Persons GetPerson(Apitoken authToken, string personId, bool detail = false)
        {
            ServicePointManager.Expect100Continue = false;

            string apiUrl = AppConfiguration.APIBaseUrl.TrimEnd('/');
            authToken.ClientSecret = AppConfiguration.ConsumerSecret;

            OAuthParameters parameters = new OAuthParameters();
            parameters.AuthToken = authToken;

            var urlParams = new Dictionary<string, string>();
            if (detail)
                urlParams.Add("request", Uri.EscapeDataString("persons/readdetails"));
            else
                urlParams.Add("request", Uri.EscapeDataString("persons/read"));

            string signatureString = parameters.CreateSignature(apiUrl, WebRequestMethods.Http.Post.ToString(), urlParams);
            string authorizationHeaderParams = parameters.CreateAuthorizationHeaderParameter(signatureString);

            string personUrl = apiUrl + "?request=persons/read";
            if (detail)
            {
                personUrl = apiUrl + "?request=persons/readdetails";
            }

            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(personUrl);


            objRequest.Method = WebRequestMethods.Http.Post;
            objRequest.Headers.Add("Authorization", authorizationHeaderParams);

            try
            {
                var authClient = new { apiCallSource = AppConfiguration.APICallSource, consumer_key = AppConfiguration.ConsumerKey, personId = personId };

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
                                log.Error("Error in method GetPerson: " + json);
                            }

                            Persons persons = JsonConvert.DeserializeObject<Persons>(json);

                            reader.Close();
                            responseStream.Close();

                            return persons;
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

        public static Rootobject GetPersonMembership(Apitoken authToken, string personId)
        {
            ServicePointManager.Expect100Continue = false;

            string apiUrl = AppConfiguration.APIBaseUrl.TrimEnd('/');
            authToken.ClientSecret = AppConfiguration.ConsumerSecret;

            OAuthParameters parameters = new OAuthParameters();
            parameters.AuthToken = authToken;

            var urlParams = new Dictionary<string, string>();
            urlParams.Add("request", Uri.EscapeDataString("persons/readmemberships"));

            string signatureString = parameters.CreateSignature(apiUrl, WebRequestMethods.Http.Post.ToString(), urlParams);
            string authorizationHeaderParams = parameters.CreateAuthorizationHeaderParameter(signatureString);

            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(apiUrl + "?request=persons/readmemberships");

            objRequest.Method = WebRequestMethods.Http.Post;
            objRequest.Headers.Add("Authorization", authorizationHeaderParams);

            try
            {
                var authClient = new { apiCallSource = AppConfiguration.APICallSource, consumer_key = AppConfiguration.ConsumerKey, personId = personId };

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
                                log.Error("Error in method GetPerson: " + json);
                            }

                            Rootobject personMembership = JsonConvert.DeserializeObject<Rootobject>(json);

                            reader.Close();
                            responseStream.Close();

                            return personMembership;
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

        public static AddPerson AddPerson(Apitoken authToken, PersonField personField)
        {
            ServicePointManager.Expect100Continue = false;

            string apiUrl = AppConfiguration.APIBaseUrl.TrimEnd('/');
            authToken.ClientSecret = AppConfiguration.ConsumerSecret;

            OAuthParameters parameters = new OAuthParameters();
            parameters.AuthToken = authToken;

            var urlParams = new Dictionary<string, string>();
            urlParams.Add("request", Uri.EscapeDataString("persons/add"));

            string signatureString = parameters.CreateSignature(apiUrl, WebRequestMethods.Http.Post.ToString(), urlParams);
            string authorizationHeaderParams = parameters.CreateAuthorizationHeaderParameter(signatureString);

            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(apiUrl + "?request=persons/add");
            objRequest.Method = WebRequestMethods.Http.Post;
            objRequest.Headers.Add("Authorization", authorizationHeaderParams);

            try
            {
                string addPersonJson = string.Empty;
                using (var requestStream = objRequest.GetRequestStream())
                {
                    addPersonJson = JsonConvert.SerializeObject(personField, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                    var bytes = Encoding.ASCII.GetBytes(addPersonJson);
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
                                log.Error("Error in method AddPerson: " + json + Environment.NewLine + " Edit Person Detail: " + addPersonJson);
                                return null;
                            }

                            try
                            {
                                AddPerson person = JsonConvert.DeserializeObject<AddPerson>(json);
                                return person;
                            }
                            catch { }

                            if(json.Length > 0)
                            {
                                AddPerson person = new Entities.AddPerson();
                                person.personId = json.Replace("\"", "");

                                return person;
                            }

                            reader.Close();
                            responseStream.Close();
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

        public static EditPerson EditPerson(Apitoken authToken, PersonFieldMultiple personField)
        {
            ServicePointManager.Expect100Continue = false;

            string apiUrl = AppConfiguration.APIBaseUrl.TrimEnd('/');
            authToken.ClientSecret = AppConfiguration.ConsumerSecret;

            OAuthParameters parameters = new OAuthParameters();
            parameters.AuthToken = authToken;

            var urlParams = new Dictionary<string, string>();
            urlParams.Add("request", Uri.EscapeDataString("persons/editmultiple"));

            string signatureString = parameters.CreateSignature(apiUrl, WebRequestMethods.Http.Post.ToString(), urlParams);
            string authorizationHeaderParams = parameters.CreateAuthorizationHeaderParameter(signatureString);

            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(apiUrl + "?request=persons/editmultiple");
            objRequest.Method = WebRequestMethods.Http.Post;
            objRequest.Headers.Add("Authorization", authorizationHeaderParams);

            try
            {
                string editPersonJson = string.Empty;
                using (var requestStream = objRequest.GetRequestStream())
                {
                    editPersonJson = JsonConvert.SerializeObject(personField, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                    var bytes = Encoding.ASCII.GetBytes(editPersonJson);
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

                            EditPerson person = JsonConvert.DeserializeObject<EditPerson>(json);

                            if (person != null && person.status.Any(X => !string.IsNullOrWhiteSpace(X.error)))
                            {
                                log.Error("Error in method EditPerson: " + json + Environment.NewLine + " Edit Person Detail: " + editPersonJson);
                            }

                            reader.Close();
                            responseStream.Close();

                            return person;
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


        public static Unbounce UnbouncePerson(Apitoken authToken, string personId)
        {
            ServicePointManager.Expect100Continue = false;

            string apiUrl = AppConfiguration.APIBaseUrl.TrimEnd('/');
            authToken.ClientSecret = AppConfiguration.ConsumerSecret;

            OAuthParameters parameters = new OAuthParameters();
            parameters.AuthToken = authToken;

            var urlParams = new Dictionary<string, string>();
            urlParams.Add("request", Uri.EscapeDataString("persons/unbounce"));

            string signatureString = parameters.CreateSignature(apiUrl, WebRequestMethods.Http.Post.ToString(), urlParams);
            string authorizationHeaderParams = parameters.CreateAuthorizationHeaderParameter(signatureString);

            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(apiUrl + "?request=persons/unbounce");
            objRequest.Method = WebRequestMethods.Http.Post;
            objRequest.Headers.Add("Authorization", authorizationHeaderParams);

            try
            {
                var authClient = new { apiCallSource = AppConfiguration.APICallSource, consumer_key = AppConfiguration.ConsumerKey, personId = personId };

                using (var requestStream = objRequest.GetRequestStream())
                {
                    var authJson = JsonConvert.SerializeObject(authClient, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

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
                                log.Error("Error in method UnbouncePerson: " + json + " PersonId: " + personId);
                            }

                            Unbounce Unbounce = JsonConvert.DeserializeObject<Unbounce>(json);

                            reader.Close();
                            responseStream.Close();

                            return Unbounce;
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

        public static AddPerson DeletePerson(Apitoken authToken, string personId)
        {
            ServicePointManager.Expect100Continue = false;

            string apiUrl = AppConfiguration.APIBaseUrl.TrimEnd('/');
            authToken.ClientSecret = AppConfiguration.ConsumerSecret;

            OAuthParameters parameters = new OAuthParameters();
            parameters.AuthToken = authToken;

            var urlParams = new Dictionary<string, string>();
            urlParams.Add("request", Uri.EscapeDataString("persons/delete"));

            string signatureString = parameters.CreateSignature(apiUrl, WebRequestMethods.Http.Post.ToString(), urlParams);
            string authorizationHeaderParams = parameters.CreateAuthorizationHeaderParameter(signatureString);

            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(apiUrl + "?request=persons/delete");
            objRequest.Method = WebRequestMethods.Http.Post;
            objRequest.Headers.Add("Authorization", authorizationHeaderParams);

            try
            {
                var authClient = new { apiCallSource = AppConfiguration.APICallSource, consumer_key = AppConfiguration.ConsumerKey, personId = personId };

                using (var requestStream = objRequest.GetRequestStream())
                {
                    var authJson = JsonConvert.SerializeObject(authClient, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

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
                                log.Error("Error in method DeletePerson: " + json + " PersonId: " + personId);
                            }

                            AddPerson person = JsonConvert.DeserializeObject<AddPerson>(json);

                            reader.Close();
                            responseStream.Close();

                            return person;
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
