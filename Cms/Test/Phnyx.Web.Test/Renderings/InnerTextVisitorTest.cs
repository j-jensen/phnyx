using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Phnyx.Web.Renderings;

namespace Phnyx.Web.Renderings.Test
{
    [TestClass]
    public class InnerTextVisitorTest
    {
        [TestMethod]
        public void ShouldReplaceRenderingsInnertext()
        {
            var writer = new StringWriter();
            var dom = "<div><div></div><div id=\"guid\"></div></div>".ToRendering();
            var sut = new InnerTextVisitor("Test string");
            sut.Filter = Filter.ElementId("guid");

            dom.Accept(sut);

            dom.Process(writer);
            Assert.AreEqual("<div><div></div><div id=\"guid\">Test string</div></div>", writer.ToString());
        }

        [TestMethod]
        public void EmptyFilterShouldReplaceRoot()
        {
            var writer = new StringWriter();
            var dom = "<div><div></div><div></div></div>".ToRendering();
            var sut = new InnerTextVisitor("Test string");

            dom.Accept(sut);

            dom.Process(writer);
            Assert.AreEqual("<div>Test string</div>", writer.ToString());
        }
    }
}
