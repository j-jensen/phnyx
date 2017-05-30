using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web;

namespace Phnyx.Web.Security.OAuth
{
    public class Facebook : Session
    {
        private static string AUTHORIZE_URI = "https://www.facebook.com/v2.9/dialog/oauth";
        private static string EXCHANGE_URI = "https://graph.facebook.com/oauth/access_token";
        private static string CLIENT_ID = ConfigurationManager.AppSettings["Phnyx.Web.Security.OAuth.Facebook.ClientID"];
        private static string CLIENT_SECRET = ConfigurationManager.AppSettings["Phnyx.Web.Security.OAuth.Facebook.ClientSecret"];

        public Facebook()
            : this(Nonce.Create(HttpContext.Current, "Facebook"))
        {
        }
        public Facebook(Nonce nonce)
        {
            this.nonce = nonce;
        }

        public override Task<UserInfo> GetUserAsync(Action<int, string> onError)
        {
            return Task.Factory.StartNew(() =>
            {
                if (string.IsNullOrEmpty(this.access_token) && !RequestAccessToken(this.request_token, onError))
                    return null;
                try
                {
                    var result = RequestApiUrl("https://graph.facebook.com/me");
                    var me = (JObject)result;

                    if (!me.Get<bool>("verified"))
                        throw new ArgumentException("Email address not verified by Facebook");

                    return new UserInfo
                    {
                        Provider = "Facebook",
                        Email = me.Get<string>("email"),
                        Firstname = me.Get<string>("first_name"),
                        Lastname = me.Get<string>("last_name"),
                        Name = me.Get<string>("name"),
                        ID = me.Get<string>("id"),
                        Locale = me.Get<string>("locale"),
                        Gender = me.Get<string>("gender").SameAs("male") ? Gender.Male : Gender.Female
                    };
                }
                catch (Exception ex)
                {
                    onError(Session.Errors.REQUEST_API_URL, ex.Message);
                    return null;
                }
            });
        }

        public static string GetAuthUrl()
        {
            var nonce = Nonce.Create(HttpContext.Current, "Facebook");
            return string.Join("?", AUTHORIZE_URI, string.Join("&",
                "client_id=" + CLIENT_ID,
                "redirect_uri=" + REDIRECT_URI,
                "state=" + nonce,
                "scope=email,public_profile"
                ));
        }

        private bool RequestAccessToken(string request_token, Action<int, string> onError)
        {
            var parameters = string.Join("&",
                "client_id=" + CLIENT_ID,
                "redirect_uri=" + REDIRECT_URI,
                "client_secret=" + CLIENT_SECRET,
                "code=" + request_token
            );

            var body = HttpPost(EXCHANGE_URI, parameters);
            if (body.Contains("error"))
            {
                onError(Session.Errors.REQUEST_ACCESS_TOKEN, body);
                return false;
            }

            var HttpResponse = JsonConvert.DeserializeObject<JObject>(body);
            this.access_token = HttpResponse["access_token"].Value<string>();
            return !string.IsNullOrEmpty(this.access_token);
        }
    }
}
