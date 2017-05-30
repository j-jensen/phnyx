using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace Phnyx.Web.Security.OAuth
{
    public class CallbackHandler : HttpTaskAsyncHandler
    {
        private static string ERROR_URL = ConfigurationManager.AppSettings["Phnyx.Web.Security.OAuth.ErrorUrl"];

        public override async Task ProcessRequestAsync(HttpContext context)
        {
            try
            {
                var qs = context.Request.QueryString;
                if (qs["error"] != null)
                {
                    OnError(context,Session.Errors.PROVIDER_ERROR, qs["error"]);
                    return;
                }
                if (qs["code"] == null)
                {
                    // Not a OAuth request!
                    OnError(context, Session.Errors.REQUEST_INVALID, "This is not a OAuth request");
                    return;
                }

                Nonce nonce = Nonce.Deserialize(HttpUtility.UrlDecode(qs["state"]));
                if (nonce.IsValid)
                {
                    var session = Session.Create(nonce, HttpUtility.UrlDecode(qs["code"]));
                    UserInfo user = await session.GetUserAsync((c, m) => OnError(context, c, m));

                    if (user != null)
                    {
                        ConnectResult connect;
                        switch (connect = Authentication.Connect(user))
                        {
                            case ConnectResult.OK:
                                Redirect(context, context.Request.Params["HTTP_REFERER"] ?? "~/");
                                return;
                            default:
                                OnError(context, Session.Errors.USER_UNAUTHORIZED, connect.ToString());
                                return;
                        }

                    }
                    else
                    {
                        // User refused to give permissions
                        OnError(context, Session.Errors.USER_DECLINED_ACCESS, "User declined access");
                        return;
                    }
                }
                else
                {
                    // Nonce is invalid try again?
                    OnError(context, Session.Errors.NONCE_INVALID, "");
                    return;
                }
            }
            catch (Exception ex)
            {
                OnError(context, Session.Errors.UNHANDLED_EXCEPTION, string.Format("{0}\n{1}", ex.Message, ex.StackTrace));
            }
        }

        private void Redirect(HttpContext context, string url, string message=null)
        {
            if (message != null)
                context.Response.AddHeader("PHNYX-MESSAGE", message);
            context.Response.StatusCode = (int)HttpStatusCode.Redirect;
            context.Response.RedirectLocation = context.Response.ApplyAppPathModifier(url);
        }
        public virtual void OnError(HttpContext context, int errorCode, string message)
        {
            var url = ERROR_URL ?? context.Request.Params["HTTP_REFERER"];
            this.Redirect(context, url + (url.Contains('?') ? "&" : "?") + "error=" + errorCode, message );
        }

        public override bool IsReusable
        {
            get { return true; }
        }
    }
}
