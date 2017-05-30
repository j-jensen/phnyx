using System;
using System.Web;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Phnyx.Web.Cms
{
    /// <summary>
    /// Module responsible for mapping the IHttpHandler to the current request
    /// </summary>
    public class ControllerModule : IHttpModule
    {
        internal static readonly object mappedHandlerKey = new object();
        internal IHandlerFactory handlerFactory;

        public ControllerModule(IHandlerFactory handlerFactory)
        {
            this.handlerFactory = handlerFactory;
        }

        public ControllerModule() : this(new StaticHandlerFactory()) { }

        public void BeginRequest(HttpContextBase context)
        {
            IHttpHandler handler;

            if ((handler = handlerFactory.GetHandler(context)) != null)
            {
                context.Items[mappedHandlerKey] = handler;
            }
        }

        public void PostMapRequestHandler(HttpContextBase context)
        {
            if (context.Items.Contains(mappedHandlerKey))
                context.Handler = context.Items[mappedHandlerKey] as IHttpHandler;
        }

        #region IHttpModule Members
        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(OnBeginRequest);
            context.PostMapRequestHandler += new EventHandler(OnPostMapRequestHandler);
        }

        void OnBeginRequest(object sender, EventArgs e)
        {
            var application = sender as HttpApplication;
            BeginRequest(new HttpContextWrapper(application.Context));
        }

        void OnPostMapRequestHandler(object sender, EventArgs e)
        {
            var application = sender as HttpApplication;
            PostMapRequestHandler(new HttpContextWrapper(application.Context));
        }

        public void Dispose()
        {
        }
        #endregion
    }

}