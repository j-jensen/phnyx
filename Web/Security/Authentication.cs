using Phnyx.Web.Security.Crypto;
using System;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace Phnyx.Web.Security
{
    public static class Authentication
    {
        public static string COOKIE_NAME = ConfigurationManager.AppSettings["Phnyx.Web.Security.Authentication.CookieName"] ?? "PHNYX_AUTH";
        private static int CACHE_TIMEOUT_MIN = 30;
        private static bool INITIALIZED;
        private static object LOCK = new object();

        public static void Initialize()
        {
            if (INITIALIZED) return;
            lock (LOCK)
            {
                if (INITIALIZED) return;
                var typename = ConfigurationManager.AppSettings["Phnyx.Web.Security.Authentication.AuthenticationBridge"];
                if (typename == null)
                    throw new ConfigurationErrorsException("Missing AppSettings key: Phnyx.Web.Security.Authentication.AuthenticationBridge");

                BRIDGE = (AuthenticationBridge)Activator.CreateInstance(Type.GetType(typename));
                INITIALIZED = true;
            }
        }

        internal static bool AuthorizeContext(HttpContext context)
        {
            Authentication.Initialize();
            // Først tjekker vi om der er en cookie vi har lagt tidligere
            var cookie = context.Request.Cookies.Get(COOKIE_NAME);
            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            {
                // Hvis cachen ikke er blevet clearet, kan vi hente login fra cachen
                var login = HttpContext.Current.Cache.Get(cookie.Value) as AuthenticationTicket;
                if (login == null)
                {
                    // Vi dekrypterer hvis cachen var tom
                    login = DecryptLogin(cookie_value: cookie.Value);
                }

                if (login != null)
                {
                    // Vi cacher login
                    context.Cache.Add(cookie.Value,
                        login,
                        null,
                        System.Web.Caching.Cache.NoAbsoluteExpiration,
                        TimeSpan.FromMinutes(CACHE_TIMEOUT_MIN),
                        System.Web.Caching.CacheItemPriority.Normal,
                        null);

                    context.User = new GenericPrincipal(new GenericIdentity(login.Name, login.Provider), login.Roles);
                    return true;
                }
                else
                {
                    context.Response.Cookies.Add(new HttpCookie(Authentication.COOKIE_NAME) { Expires = DateTime.Today });
                }
            }
            return false;
        }

        public static ConnectResult Connect(UserInfo user)
        {
            Authentication.Initialize();
            var login = Authentication.Bridge.Authenticate(user.Provider, user.ID);
            switch (login.Result)
            {
                case ConnectResult.OK:
                    SetAuthCookie(login.Ticket);
                    break;
                case ConnectResult.UnknownId:
                    login = Authentication.Bridge.CreateAndAuthenticate(user.Provider, user.ID, user.Email, user.Name, user.Firstname, user.Lastname, user.Gender);
                    if (login.Result == ConnectResult.OK)
                        goto case ConnectResult.OK;
                    break;
            }
            return login.Result;
        }

        public static AuthenticationTicket GetTicket(HttpContext context)
        {
            Authentication.Initialize();
            // Først tjekker vi om der er en cookie vi
            AuthenticationTicket login = null;

            var cookie = context.Request.Cookies.Get(COOKIE_NAME);
            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            {
                // Hvis cachen ikke er blevet clearet, kan vi hente login fra cachen
                login = HttpContext.Current.Cache.Get(cookie.Value) as AuthenticationTicket;
                if (login == null)
                {
                    // Vi dekrypterer hvis cachen var tom
                    login = DecryptLogin(cookie_value: cookie.Value);
                }
            }
            return login;
        }

        /// <summary>
        /// Writes the encrypted login to the clients cookie, and hereby authenticate subsequent requests
        /// </summary>
        private static void SetAuthCookie(AuthenticationTicket login)
        {
            string encTicket = Encrypter.Encrypt(login.Stringify());
            var cookie = new HttpCookie(COOKIE_NAME, encTicket)
            {
                HttpOnly = true
            };
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        private static AuthenticationTicket DecryptLogin(string cookie_value)
        {
            AuthenticationTicket login = null;
            try
            {
                var stringified_login = Encrypter.Decrypt(cookie_value);

                login = AuthenticationTicket.Parse(stringified_login);
            }
            catch
            {
            }
            return login;
        }

        private static AuthenticationBridge BRIDGE;
        public static AuthenticationBridge Bridge
        {
            get
            {
                Authentication.Initialize();
                if (BRIDGE == null)
                    throw new InvalidOperationException("AuthenticationBridge not found");
                return BRIDGE;
            }
        }
    }

    public enum ConnectResult : int
    {
        UnknownId = 0x01,
        UnknownProvider = 0x02,
        OK = 0x04,
        UserIsLocked = 0x08
    }
}