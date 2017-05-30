using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Phnyx.Web.Renderings;

namespace Phnyx.Web.Renderings.Test
{
    public static class HtmlExtension
    {
        public static RenderingBase ToRendering(this string html)
        {
            return HtmlParser.ParseString(html);
        }
    }
}
