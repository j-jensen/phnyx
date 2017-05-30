using System;
using System.Web;

namespace Phnyx.Web.Security
{
    public class AuthenticationModule : IHttpModule
    {
        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.AuthenticateRequest += delegate(object sender, EventArgs e)
            {
                var app = sender as HttpApplication;

                Authentication.AuthorizeContext(app.Context);
            };
        }

    }
}