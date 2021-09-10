using Newtonsoft.Json;
using SwiftDigitalSyncManager.Entities;
using SwiftDigitalSyncManager.Helpers;
using SwiftDigitalSyncManager.OAuth;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SwiftDigitalSyncManager.SwiftDigitalApi
{
    public class GroupManager
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static Groups GetMailGroups(Apitoken authToken)
        {
            string apiUrl = AppConfiguration.APIBaseUrl.TrimEnd('/');
            authToken.ClientSecret = AppConfiguration.ConsumerSecret;

            OAuthParameters parameters = new OAuthParameters();
            parameters.AuthToken = authToken;

            var urlParams = new Dictionary<string, string>();
            urlParams.Add("request", Uri.EscapeDataString("mailgroups/readall"));

            string signatureString = parameters.CreateSignature(apiUrl, WebRequestMethods.Http.Post.ToString(), urlParams);
            string authorizationHeaderParams = parameters.CreateAuthorizationHeaderParameter(signatureString);

            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(apiUrl + "?request=mailgroups/readall");
            objRequest.Method = WebRequestMethods.Http.Post;
            objRequest.Headers.Add("Authorization", authorizationHeaderParams);

            try
            {
                var authClient = new { apiCallSource = AppConfiguration.APICallSource, consumer_key = AppConfiguration.ConsumerKey };

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
                                log.Error("Error in method GetMailGroups: " + json);
                            }

                            Groups groups = JsonConvert.DeserializeObject<Groups>(json);

                            reader.Close();
                            responseStream.Close();

                            return groups;
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

        public static Membersdetail GetGroupMembers(Apitoken authToken, string groupId)
        {
            string apiUrl = AppConfiguration.APIBaseUrl.TrimEnd('/');
            authToken.ClientSecret = AppConfiguration.ConsumerSecret;

            OAuthParameters parameters = new OAuthParameters();
            parameters.AuthToken = authToken;

            var urlParams = new Dictionary<string, string>();
            urlParams.Add("request", Uri.EscapeDataString("mailgroups/readmembersdetails"));

            string signatureString = parameters.CreateSignature(apiUrl, WebRequestMethods.Http.Post.ToString(), urlParams);
            string authorizationHeaderParams = parameters.CreateAuthorizationHeaderParameter(signatureString);

            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(apiUrl + "?request=mailgroups/readmembersdetails");
            objRequest.Method = WebRequestMethods.Http.Post;
            objRequest.Headers.Add("Authorization", authorizationHeaderParams);

            try
            {
                var authClient = new { apiCallSource = AppConfiguration.APICallSource, consumer_key = AppConfiguration.ConsumerKey, mailGroupId = groupId };

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
                                log.Error("Error in method GetGroupMembers: " + json);
                            }

                            Membersdetail groups = JsonConvert.DeserializeObject<Membersdetail>(json);

                            reader.Close();
                            responseStream.Close();

                            return groups;
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
        
        public static AddGroupMember AddMemeberToGroup(Apitoken authToken, string groupId, string personId)
        {
            string apiUrl = AppConfiguration.APIBaseUrl.TrimEnd('/');
            authToken.ClientSecret = AppConfiguration.ConsumerSecret;

            OAuthParameters parameters = new OAuthParameters();
            parameters.AuthToken = authToken;

            var urlParams = new Dictionary<string, string>();
            urlParams.Add("request", Uri.EscapeDataString("mailgroups/addmember"));

            string signatureString = parameters.CreateSignature(apiUrl, WebRequestMethods.Http.Post.ToString(), urlParams);
            string authorizationHeaderParams = parameters.CreateAuthorizationHeaderParameter(signatureString);

            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(apiUrl + "?request=mailgroups/addmember");
            objRequest.Method = WebRequestMethods.Http.Post;
            objRequest.Headers.Add("Authorization", authorizationHeaderParams);

            try
            {
                var authClient = new { apiCallSource = AppConfiguration.APICallSource, consumer_key = AppConfiguration.ConsumerKey, personId = personId, mailGroupId = groupId };

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
                                log.Error("Error in method AddMemeberToGroup: " + json + " PersonId: " + personId + ", MailGroupId: " + groupId);
                            }

                            AddGroupMember addMemberStatus = JsonConvert.DeserializeObject<AddGroupMember>(json);

                            reader.Close();
                            responseStream.Close();

                            return addMemberStatus;
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

        public static AddGroupMember AddMemeberToGroupMultiple(Apitoken authToken, string groupId, string[] personIds)
        {
            string apiUrl = AppConfiguration.APIBaseUrl.TrimEnd('/');
            authToken.ClientSecret = AppConfiguration.ConsumerSecret;

            OAuthParameters parameters = new OAuthParameters();
            parameters.AuthToken = authToken;

            var urlParams = new Dictionary<string, string>();
            urlParams.Add("request", Uri.EscapeDataString("mailgroups/addmembermultiple"));

            string signatureString = parameters.CreateSignature(apiUrl, WebRequestMethods.Http.Post.ToString(), urlParams);
            string authorizationHeaderParams = parameters.CreateAuthorizationHeaderParameter(signatureString);

            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(apiUrl + "?request=mailgroups/addmembermultiple");
            objRequest.Method = WebRequestMethods.Http.Post;
            objRequest.Headers.Add("Authorization", authorizationHeaderParams);

            try
            {
                var authClient = new { apiCallSource = AppConfiguration.APICallSource, consumer_key = AppConfiguration.ConsumerKey, personId = string.Join(",", personIds), mailGroupId = groupId };

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
                                log.Error("Error in method AddMemeberToGroup: " + json + " PersonId: " + string.Join(",", personIds) + ", MailGroupId: " + groupId);
                            }

                            AddGroupMember addMemberStatus = JsonConvert.DeserializeObject<AddGroupMember>(json);

                            reader.Close();
                            responseStream.Close();

                            return addMemberStatus;
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

        public static AddGroupMember RemoveMemeberFromGroup(Apitoken authToken, string groupId, string[] personIds)
        {
            string apiUrl = AppConfiguration.APIBaseUrl.TrimEnd('/');
            authToken.ClientSecret = AppConfiguration.ConsumerSecret;

            OAuthParameters parameters = new OAuthParameters();
            parameters.AuthToken = authToken;

            var urlParams = new Dictionary<string, string>();
            urlParams.Add("request", Uri.EscapeDataString("mailgroups/removemember"));

            string signatureString = parameters.CreateSignature(apiUrl, WebRequestMethods.Http.Post.ToString(), urlParams);
            string authorizationHeaderParams = parameters.CreateAuthorizationHeaderParameter(signatureString);

            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(apiUrl + "?request=mailgroups/removemember");
            objRequest.Method = WebRequestMethods.Http.Post;
            objRequest.Headers.Add("Authorization", authorizationHeaderParams);

            try
            {
                var authClient = new { apiCallSource = AppConfiguration.APICallSource, consumer_key = AppConfiguration.ConsumerKey, personId = string.Join(",", personIds), mailGroupId = groupId };

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
                                log.Error("Error in method RemoveMemeberFromGroup: " + json + " PersonIds: " + string.Join(",", personIds) + ", MailGroupId: " + groupId);
                            }

                            AddGroupMember addMemberStatus = JsonConvert.DeserializeObject<AddGroupMember>(json);

                            reader.Close();
                            responseStream.Close();

                            return addMemberStatus;
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

        public static AddGroupMember RemoveMemeberFromGroupMultiple(Apitoken authToken, string groupId, string personId)
        {
            string apiUrl = AppConfiguration.APIBaseUrl.TrimEnd('/');
            authToken.ClientSecret = AppConfiguration.ConsumerSecret;

            OAuthParameters parameters = new OAuthParameters();
            parameters.AuthToken = authToken;

            var urlParams = new Dictionary<string, string>();
            urlParams.Add("request", Uri.EscapeDataString("mailgroups/removemembermultiple"));

            string signatureString = parameters.CreateSignature(apiUrl, WebRequestMethods.Http.Post.ToString(), urlParams);
            string authorizationHeaderParams = parameters.CreateAuthorizationHeaderParameter(signatureString);

            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(apiUrl + "?request=mailgroups/removemembermultiple");
            objRequest.Method = WebRequestMethods.Http.Post;
            objRequest.Headers.Add("Authorization", authorizationHeaderParams);

            try
            {
                var authClient = new { apiCallSource = AppConfiguration.APICallSource, consumer_key = AppConfiguration.ConsumerKey, personId = personId, mailGroupId = groupId };

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
                                log.Error("Error in method RemoveMemeberFromGroup: " + json + " PersonId: " + personId + ", MailGroupId: " + groupId);
                            }

                            AddGroupMember addMemberStatus = JsonConvert.DeserializeObject<AddGroupMember>(json);

                            reader.Close();
                            responseStream.Close();

                            return addMemberStatus;
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
