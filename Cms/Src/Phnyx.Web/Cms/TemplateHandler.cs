using System.Web;
using System;
using Phnyx.Web.Renderings;
using System.Diagnostics.CodeAnalysis;

namespace Phnyx.Web.Cms
{
    public class TemplateHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            template.Bind(Data);

            template.Process(context.Response.Output);
        }

        public object Data { get; set; }

        private Template template;
        public Template Template
        {
            get
            {
                return template;
            }
            set
            {
                template = value;
            }
        }
    }
}