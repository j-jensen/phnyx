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
    public class HtmlTagRenderingTest
    {
        [TestMethod]
        public void ShoudBeOfTypeRendering()
        {
            var sut = new HtmlTag();
            Assert.IsInstanceOfType(sut, typeof(RenderingBase));
        }

        [TestMethod]
        public void ShoudHaveTagnameProperty()
        {
            var sut = new HtmlTag();

            Assert.AreEqual("div", sut.Name);
        }

        [TestMethod]
        public void ShoudHaveIsFullTagProperty()
        {
            var sut = new HtmlTag();

            Assert.AreEqual(false, sut.IsFullTag);
        }

        [TestMethod]
        public void ShoudRenderContentInsideHtmltag()
        {
            var sut = new HtmlTag();
            sut.Add(new Literal("indhold"));
            var writer = new StringWriter();

            sut.Process(writer);

            Assert.AreEqual("<div>indhold</div>", writer.ToString());
        }

        [TestMethod]
        public void ShoudNotRenderContentInsideHtmltagIfIsFullTagIsTrue()
        {
            var sut = new HtmlTag();
            sut.Add(new Literal("indhold"));
            sut.IsFullTag = true;
            var writer = new StringWriter();

            sut.Process(writer);

            Assert.AreEqual("<div />", writer.ToString());
        }

        [TestMethod]
        public void ShoudRenderAttributes()
        {
            var sut = new HtmlTag();
            sut.Attributes["id"] = "myName";
            var writer = new StringWriter();

            sut.Process(writer);

            Assert.AreEqual("<div id=\"myName\"></div>", writer.ToString());
        }
    }
}
