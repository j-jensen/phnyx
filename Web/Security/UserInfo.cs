using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phnyx.Web.Security
{
    public class UserInfo
    {
        public string ID { get; set; }
        public Gender Gender { get; set; }
        public string Locale { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Provider { get; set; }
    }

    public enum Gender
    {
        Unknown, Male, Female
    }
}
