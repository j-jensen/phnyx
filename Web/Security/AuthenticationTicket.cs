
using System;
using System.Web.UI;
namespace Phnyx.Web.Security
{
    [Serializable]
    public class AuthenticationTicket
    {
        public string Provider { get; set; }
        public string Id { get; set; }
        public string[] Roles { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }

        public string Stringify()
        {
            var tw = new System.IO.StringWriter();
            var los = new LosFormatter();
            los.Serialize(tw, this);
            return tw.ToString();
        }

        public static AuthenticationTicket Parse(string stringified_login)
        {
            return (AuthenticationTicket)new LosFormatter().Deserialize(stringified_login);
        }
    }
}
