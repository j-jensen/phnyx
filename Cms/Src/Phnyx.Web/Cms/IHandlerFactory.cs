using System;
using System.Web;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Phnyx.Web.Cms
{
    public interface IHandlerFactory
    {
        IHttpHandler GetHandler(HttpContextBase context);
    }
}
