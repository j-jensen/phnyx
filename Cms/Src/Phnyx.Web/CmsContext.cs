using System;
using System.Web;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;

namespace Phnyx.Web
{
    public class CmsContext
    {
        private readonly HttpContext _httpContext;

        public CmsContext(HttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        public string MapPath(string path)
        {
            return _httpContext.Request.MapPath(path);
        }

        public TextWriter Output
        {
            get
            {
                return _httpContext.Response.Output;
            }
        }
        
        public string RequestedPath
        {
            get
            {
                return _httpContext.Request.Path;
            }
        }
        public CmsHandler Handler { get; set; }
    }
}
