using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ubidots
{
    public class ServerBridge
    {
        /* Constant configuration variables */
        public const string DEFAULT_BASE_URL = "https://things.ubidots.com/api/";
        public const string DEFAULT_BASE_VERSION = "v1.6";

        /* Instance variables */
        private string BaseUrl;
        private string ApiKey;
        private string Token;
        private WebHeaderCollection TokenHeader;
        private WebHeaderCollection ApiKeyHeader;

        public ServerBridge(string ApiKey) : this(ApiKey, DEFAULT_BASE_URL, DEFAULT_BASE_VERSION) {}

        public ServerBridge(string ApiKey, string BaseUrl) : this(ApiKey, BaseUrl, DEFAULT_BASE_VERSION) {}

        public ServerBridge(string ApiKey, string BaseUrl, string BaseVersion)
        {
            this.ApiKey = ApiKey;
            this.BaseUrl = BaseUrl + BaseVersion + "/";
            Token = null;

            ApiKeyHeader = new WebHeaderCollection();
            ApiKeyHeader.Add("X-UBIDOTS-APIKEY", this.ApiKey);

            Init();
        }

        /// <summary>
        ///  Initiates the reception of the token from Ubidots
        /// </summary>
        public void Init()
        {
            ReceiveToken();
        }

        /// <summary>
        /// Receives the token from Ubidots and save it in Token variable
        /// </summary>
        private void ReceiveToken()
        {
            Token = (string) JObject.Parse(PostWithApiKey("auth/token")).GetValue("token");

            TokenHeader = new WebHeaderCollection();
            TokenHeader.Add("X-AUTH-TOKEN", Token);
        }

        /// <summary>
        /// Prepare the headers for the HTTP Request
        /// </summary>
        /// <param name="CustomHeaders" /> The headers to combine in one WebHeaderCollection
        /// <returns>The combination of the headers to be sent to Ubidots</returns>
        private WebHeaderCollection PrepareHeaders(WebHeaderCollection CustomHeaders)
        {
            WebHeaderCollection Headers = new WebHeaderCollection();

            Headers.Add(CustomHeaders);

            return Headers;
        }

        /// <summary>
        /// Send a POST request to Ubidots using the API Key
        /// Generally this is used for getting the token, nothing else.
        /// </summary>
        /// <param name="Path" /> The path were we're going to query in Ubidots API
        /// <returns>A string containing the response of the server</returns>
        private string PostWithApiKey(string Path)
        {
            string Response = null;
            WebHeaderCollection Headers = PrepareHeaders(ApiKeyHeader);

            string Url = BaseUrl + Path;
            try
            {
                HttpWebRequest Request = WebRequest.Create(Url) as HttpWebRequest;
                Request.Method = "POST";
                Request.Headers = Headers;
                Request.ContentType = "application/json";

                using (HttpWebResponse WebResponse = Request.GetResponse() as HttpWebResponse)
                {
                    using (Stream ResponseStream = WebResponse.GetResponseStream())
                    {
                        StreamReader Reader = new StreamReader(ResponseStream, Encoding.UTF8);
                        Response = Reader.ReadToEnd();
                        return Response;
                    }
                }
            } 
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Send a GET request to Ubidots using the Token
        /// </summary>
        /// <param name="Path" />  The path were we're going to query in Ubidots API
        /// <returns>A string containing the response of the server</returns>
        public string Get(string Path)
        {
            string Response = null;
            WebHeaderCollection Headers = PrepareHeaders(TokenHeader);

            string Url = BaseUrl + Path;
            try
            {
                HttpWebRequest Request = WebRequest.Create(Url) as HttpWebRequest;
                Request.Method = "GET";
                Request.Headers = Headers;
                Request.ContentType = "application/json";

                using (HttpWebResponse WebResponse = Request.GetResponse() as HttpWebResponse)
                {
                    using (Stream ResponseStream = WebResponse.GetResponseStream())
                    {
                        StreamReader Reader = new StreamReader(ResponseStream, Encoding.UTF8);
                        Response = Reader.ReadToEnd();

                        if (JObject.Parse(Response).GetValue("results") != null)
                        {
                            Response = JObject.Parse(Response).GetValue("results").ToString();
                        }

                        return Response;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Send a POST request to Ubidots using the Token
        /// </summary>
        /// <param name="Path" />  The path were we're going to query in Ubidots API
        /// <param name="Json" /> The json we're going to send to the server
        /// <returns>A string containing the response of the server</returns>
        public string Post(string Path, string Json)
        {
            string Response = null;
            WebHeaderCollection Headers = PrepareHeaders(TokenHeader);

            string Url = BaseUrl + Path;
            try
            {
                HttpWebRequest Request = WebRequest.Create(Url) as HttpWebRequest;
                Request.Method = "POST";
                Request.Headers = Headers;
                Request.ContentType = "application/json";
                Request.Accept = "application/json";

                using (StreamWriter SWriter = new StreamWriter(Request.GetRequestStream()))
                {
                    SWriter.Write(Json);
                    SWriter.Flush();
                }

                using (HttpWebResponse WebResponse = Request.GetResponse() as HttpWebResponse)
                {
                    using (Stream ResponseStream = WebResponse.GetResponseStream())
                    {
                        StreamReader Reader = new StreamReader(ResponseStream, Encoding.UTF8);
                        Response = Reader.ReadToEnd();
                        return Response;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Send a DELETE request to Ubidots using the Token
        /// </summary>
        /// <param name="Path" />  The path were we're going to query in Ubidots API
        /// <returns>A string containing the response of the server</returns>
        public string Delete(string Path)
        {
            string Response = null;
            WebHeaderCollection Headers = PrepareHeaders(TokenHeader);

            string Url = BaseUrl + Path;
            try
            {
                HttpWebRequest Request = WebRequest.Create(Url) as HttpWebRequest;
                Request.Method = "DELETE";
                Request.Headers = Headers;
                Request.ContentType = "application/json";

                using (HttpWebResponse WebResponse = Request.GetResponse() as HttpWebResponse)
                {
                    using (Stream ResponseStream = WebResponse.GetResponseStream())
                    {
                        StreamReader Reader = new StreamReader(ResponseStream, Encoding.UTF8);
                        Response = Reader.ReadToEnd();
                        return Response;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}
