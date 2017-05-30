using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phnyx
{
    internal static class Misc
    {
        public static T Get<T>(this Newtonsoft.Json.Linq.JObject container, string propertyName)
        {
            return container.Value<T>(propertyName);
        }

        public static NameValueCollection ToNameValueCollection(this object self)
        {
            var collection = new NameValueCollection();
            self.GetType().GetProperties()
                .ToList()
                .ForEach(pi =>
                {
                    var val = pi.GetValue(self, null) ?? "";
                    collection.Add(pi.Name, val.ToString());
                });
            return collection;
        }

        public static Dictionary<string, string> ToDictionary(this string self)
        {
            var dict = new Dictionary<string, string>();
            string[] tokens = null;
            if (self != null && (tokens = self.Split('|')).Length > 0)
            {
                foreach (var item in tokens)
                {
                    var parts = item.Split('=');
                    if (parts.Length > 1)
                    {
                        dict.Add(parts[0], parts[1]);
                    }
                    else
                        dict.Add(item, item);
                }
            }

            return dict;
        }

        public static object ToInfo(this NameValueCollection self)
        {
            try
            {
                return new 
                {
                    ID = self["ID"],
                    Provider = self["Provider"],
                    Email = self["email"],
                    Lastname = self["lastname"],
                    Name = self["name"],
                    PictureUrl = self["pictureurl"],
                    Firstname = self["firstname"]
                };
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Case-in-sensitive match
        /// </summary>
        public static bool SameAs(this string self, string other)
        {
            return self.Equals(other, StringComparison.OrdinalIgnoreCase);
        }
    }
}
