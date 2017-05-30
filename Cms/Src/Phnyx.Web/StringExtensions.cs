using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phnyx.Web
{
    public static class StringExtensions
    {
        public static bool EqualsIgnoreCase(this string input, string value)
        {
            return input.Equals(value, StringComparison.OrdinalIgnoreCase);
        }
    }
}
