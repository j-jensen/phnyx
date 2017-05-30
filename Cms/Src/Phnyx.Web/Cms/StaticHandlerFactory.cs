using System;
using System.Web;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Phnyx.Web.Renderings;

namespace Phnyx.Web.Cms
{
    public class StaticHandlerFactory : IHandlerFactory
    {
        public IHttpHandler GetHandler(HttpContextBase context)
        {
            var originalPath = context.Request.FilePath;

            if (originalPath.IndexOf("aspx", StringComparison.OrdinalIgnoreCase) > -1 || originalPath.IndexOf(".htm", StringComparison.OrdinalIgnoreCase) > -1)
            {
                var handler = new TemplateHandler();
                handler.Data = new
                {
                    Title = "DGI - Badminton",
                    Main = "<h1>DGI</h1><p>På denne side, kan man se en oversigt over alle de produkter der er i systemet.</p><pre>public class Foo{\n}</pre>",
                    Menu = "<ul><li><a href='#'>Punkt 1</a></li><li><a href='#'>Punkt 2</a></li><li><a href='#'>Punkt 3</a></li></ul>",
                    Slogan = "Slå ikke på manden....brug bolden!",
                    Logo = "DGI<span>badminton</span>"
                };
                handler.Template = new Template("~/Layout/index.html");

                return handler;
            }
            else
                return null;
        }
    }
}
