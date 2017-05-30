using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web;

namespace Phnyx.Web.Security.OAuth
{
    public class Google : Session
    {
        private static string AUTHORIZE_URI = "https://accounts.google.com/o/oauth2/auth";
        private static string EXCHANGE_URI = "https://accounts.google.com/o/oauth2/token";
        private static string CLIENT_ID = ConfigurationManager.AppSettings["Phnyx.Web.Security.OAuth.Google.ClientID"];
        private static string CLIENT_SECRET = ConfigurationManager.AppSettings["Phnyx.Web.Security.OAuth.Google.ClientSecret"];

        public Google()
            : this(Nonce.Create(HttpContext.Current, "Google"))
        {
        }
        public Google(Nonce nonce)
        {
            this.nonce = nonce;
        }

        public override Task<UserInfo> GetUserAsync(Action<int,string> onError)
        {
            return Task.Factory.StartNew(() =>
            {
                if (string.IsNullOrEmpty(this.access_token) && !RequestAccessToken(this.request_token, onError)) return null;

                try
                {
                    var result = RequestApiUrl("https://www.googleapis.com/oauth2/v1/userinfo");
                    var me = (JObject)result;

                    if (!me.Get<bool>("verified_email"))
                        throw new ArgumentException("Email address not verified by Google");

                    return new UserInfo
                    {
                        Provider = "Google",
                        Email = me.Get<string>("email"),
                        Firstname = me.Get<string>("given_name"),
                        Lastname = me.Get<string>("family_name"),
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
                var nonce = Nonce.Create(HttpContext.Current, "Google");

                return string.Join("?", AUTHORIZE_URI, string.Join("&",
                    "client_id=" + CLIENT_ID,
                    "redirect_uri=" + REDIRECT_URI,
                    "response_type=code",
                    "scope=https://www.googleapis.com/auth/userinfo.email https://www.googleapis.com/auth/userinfo.profile",
                    "state=" + nonce
                    ));
        }

        private bool RequestAccessToken(string request_token, Action<int, string> onError)
        {
            var parameters = string.Join("&",
                "client_id=" + CLIENT_ID,
                "redirect_uri=" + REDIRECT_URI,
                "client_secret=" + CLIENT_SECRET,
                "code=" + request_token,
                "grant_type=authorization_code"
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
