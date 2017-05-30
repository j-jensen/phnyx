using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Phnyx.Web.Security.OAuth
{
    public class MSAccount : Session
    {
        private static string AUTHORIZE_URI = "https://login.live.com/oauth20_authorize.srf";
        private static string EXCHANGE_URI = "https://login.live.com/oauth20_token.srf";
        private static string CLIENT_ID = ConfigurationManager.AppSettings["Phnyx.Web.Security.OAuth.MSAccount.ClientID"];
        private static string CLIENT_SECRET = ConfigurationManager.AppSettings["Phnyx.Web.Security.OAuth.MSAccount.ClientSecret"];

        public MSAccount()
            : this(Nonce.Create(HttpContext.Current, "MSAccount"))
        {
        }

        public MSAccount(Nonce nonce)
        {
            this.nonce = nonce;
        }

        public override Task<UserInfo> GetUserAsync(Action<int, string> onError)
        {
            return Task.Factory.StartNew(() =>
            {
                if (string.IsNullOrEmpty(this.access_token) && !RequestAccessToken(this.request_token, onError)) return null;

                try
                {
                    var result = RequestApiUrl("https://apis.live.net/v5.0/me");
                    var me = (JObject)result;

                    return new UserInfo
                    {
                        Provider = "MSAccount",
                        Email = me.Get<JObject>("emails").Get<string>("preferred"),
                        Firstname = me.Get<string>("first_name"),
                        Lastname = me.Get<string>("last_name"),
                        Name = me.Get<string>("name"),
                        ID = me.Get<string>("id"),
                        Locale = me.Get<string>("locale"),
                        Gender = Gender.Unknown
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
            var nonce = Nonce.Create(HttpContext.Current, "MSAccount");
            return string.Join("?", AUTHORIZE_URI, string.Join("&",
                 "client_id=" + CLIENT_ID,
                 "redirect_uri=" + REDIRECT_URI,
                 "response_type=code",
                 "scope=wl.signin%20wl.basic%20wl.emails",
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
