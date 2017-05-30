using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Configuration;
using Phnyx.Web.Security.Crypto;

namespace Phnyx.Web.Security
{
    public struct Nonce
    {
        private static int TIMEOUT = ConfigurationManager.AppSettings["Phnyx.Web.Security.Nonce.Timeout"] != null ? int.Parse(ConfigurationManager.AppSettings["Phnyx.Web.Security.Nonce.Timeout"]) : 120;
        public readonly string IP;
        public readonly DateTime Created;
        public readonly string Provider;
        public readonly string Url;

        internal static Func<DateTime> GET_NOW = () => DateTime.Now;
        internal static Func<string> GET_CLIENT_IP = () => HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

        internal Nonce(string ip, string url, DateTime created, string provider)
        {
            this.IP = ip;
            this.Created = created;
            this.Provider = provider;
            this.Url = url;
        }

        public static Nonce Create(HttpContext http_context, string provider)
        {
            return new Nonce(http_context.Request.UserHostAddress, http_context.Request.Headers["REFERER"], GET_NOW(), provider);
        }

        public bool IsValid
        {
            get
            {
                return this.Created >= GET_NOW().AddSeconds(-TIMEOUT) && this.IP == GET_CLIENT_IP();
            }
        }

        public static bool Validate(string encrypted_nonce, out string provider)
        {
            var nonce = Nonce.Deserialize(encrypted_nonce);
            provider = nonce.Provider;
            return nonce.IsValid;
        }

        public override string ToString()
        {
            return Serialize(this);
        }

        #region Private functions
        private static string Serialize(Nonce nonce)
        {
            var blob = string.Join(";", nonce.IP, nonce.Url, nonce.Created.ToUniversalTime().ToFileTimeUtc(), nonce.Provider);
            return Encrypter.Encrypt(blob);
        }

        public static Nonce Deserialize(string encryptedstring)
        {
            var blob = Encrypter.Decrypt(encryptedstring);
            var parts = blob.Split(';');
            if (parts.Length != 4)
                throw new ArgumentException("The decryption resulted in a corupt nonce", "encryptedstring");

            var ip = parts[0];
            var url = parts[1];
            long time;
            if (!long.TryParse(parts[2], out time))
                throw new ArgumentException("The decryption resulted in a corupt filetime", "parts[1]");
            var date = DateTime.FromFileTimeUtc(time).ToLocalTime();
            var provider = parts[3];

            return new Nonce(ip, url, date, provider);
        }
        #endregion
    }
}
