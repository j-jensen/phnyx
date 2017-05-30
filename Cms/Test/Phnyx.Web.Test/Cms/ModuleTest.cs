using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;

namespace Phnyx.Web.Cms.Test
{
    [TestClass, DeploymentItem(@".\Web\Layout\index.html")]
    public class ModuleTest
    {
        [TestMethod]
        public void BeginRequestShouldAddObjectToContextItems()
        {
            var sut = new ControllerModule(new HandlerFactoryMock());
            var context = new HttpContextMock();

            sut.BeginRequest(context);

            Assert.IsTrue(context.Items.Contains(ControllerModule.mappedHandlerKey));
        }

        [TestMethod]
        public void PostMapRequestHandlerShouldSetContextHandler()
        {
            var sut = new ControllerModule(new HandlerFactoryMock());
            var context = new HttpContextMock();

            sut.BeginRequest(context);
            sut.PostMapRequestHandler(context);

            Assert.AreEqual(context.Items[ControllerModule.mappedHandlerKey], context.Handler);
        }
    }

    #region Mocking
    class HandlerMock : IHttpHandler
    {
        public bool IsReusable
        {
            get { throw new NotImplementedException(); }
        }

        public void ProcessRequest(HttpContext context)
        {
            throw new NotImplementedException();
        }

    }
    class HandlerFactoryMock : IHandlerFactory
    {
        public IHttpHandler GetHandler(HttpContextBase context)
        {
            return new HandlerMock();
        }
    }

    class HttpContextMock : HttpContextBase
    {
        private Dictionary<object, object> _items = new Dictionary<object, object>();

        public override HttpRequestBase Request
        {
            get
            {
                return new HttpRequestMock();
            }
        }

        public override System.Collections.IDictionary Items
        {
            get
            {
                return _items;
            }
        }

        public override IHttpHandler Handler
        {
            get;
            set;
        }
    }

    class HttpRequestMock : HttpRequestBase
    {
        public override string MapPath(string virtualPath)
        {
            return System.IO.Path.GetFullPath("index.html");
        }

        public override string FilePath
        {
            get
            {
                return "/index.html";
            }
        }
    }
    
    #endregion
}
