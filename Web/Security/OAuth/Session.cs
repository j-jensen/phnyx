using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Threading.Tasks;

namespace Phnyx.Web.Security.OAuth
{
    public abstract class Session
    {
        protected static string REDIRECT_URI = ConfigurationManager.AppSettings["Phnyx.Web.Security.OAuth.CallbackUrl"];
        protected string access_token;
        protected string scope;
        protected Nonce nonce;
        protected string request_token;
        public abstract Task<UserInfo> GetUserAsync(Action<int, string> onError);

        public object RequestApiUrl(string api_url)
        {
            if (this.access_token == null)
                throw new NullReferenceException("Can't call api without access_token");

            var url = string.Join("?", api_url, "access_token=" + access_token);

            var str = HttpGet(url);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(str);
        }

        protected string HttpGet(string URI)
        {
            System.Net.WebRequest req = System.Net.WebRequest.Create(URI);
            var resp = req.GetResponse();

            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            return sr.ReadToEnd().Trim();
        }

        protected string HttpPost(string URI, string parameters)
        {
            WebRequest req = System.Net.WebRequest.Create(URI);
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(parameters);
            req.ContentLength = bytes.Length;

            using (Stream os = req.GetRequestStream())
            {
                os.Write(bytes, 0, bytes.Length);
                os.Close();
                using (System.Net.WebResponse resp = req.GetResponse())
                {
                    if (resp == null) return null;
                    System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                    return sr.ReadToEnd().Trim();
                }
            }
        }

        public static Session Create(Nonce nonce, string requestToken)
        {
            Session session = null;

            switch (nonce.Provider)
            {
                case "Facebook":
                    session = new Facebook(nonce);
                    break;
                case "Google":
                    session = new Google(nonce);
                    break;
                case "MSAccount":
                    session = new MSAccount(nonce);
                    break;
                default:
                    throw new NotImplementedException(nonce.Provider);
            }

            session.request_token = requestToken;
            return session;
        }

        public class Errors
        {
            public const int REQUEST_API_URL =      57345;
            public const int REQUEST_ACCESS_TOKEN = 57346;
            public const int USER_DECLINED_ACCESS = 57348;
            public const int UNHANDLED_EXCEPTION =  57352;
            public const int NONCE_INVALID =        57360;
            public const int USER_UNAUTHORIZED =    57376;
            public const int PROVIDER_ERROR       = 57408;
            public const int REQUEST_INVALID =      57472;
        }

    }
}