using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Phnyx.Web.Renderings;
using Phnyx.Web.Renderings.Test;

namespace Phnyx.Web.Test
{
    [TestClass]
    public class HtmlParserTest
    {
        private string HTML = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\"><html><head><title>Overskrift</title></head><body><div id=\"main\"><h1>Overskrift</h1><img src=\"jugga.jpg\" alt=\"Picture description\" /><p>Lurum lej di dej...<br />Yadiya...</p></div></body></html>";

        [TestMethod]
        public void ShouldParseDomTree()
        {
            var dom = HTML.ToRendering();

            Assert.AreEqual<int>(2, dom.Count, "Documet shoud have 2 elements in the root; doctype and html");
            Assert.IsInstanceOfType(dom[0], typeof(Literal));
            Assert.IsInstanceOfType(dom[1], typeof(HtmlTag));
            Assert.IsInstanceOfType(dom[1][0][0][0], typeof(Literal));
        }

        [TestMethod]
        public void ShouldDependOnTextReaderAndParseRendering()
        {
            var rendering = HtmlParser.Parse((TextReader)new StringReader(""));

            Assert.IsNotNull(rendering);
        }

        [TestMethod]
        public void ShouldParseNonValidTagsAsValidXhtml()
        {
            var writer = new StringWriter();
            var rendering = "<br><input type=\"text\">".ToRendering();

            rendering.Process(writer);
            Assert.AreEqual<string>("<br /><input type=\"text\" />", writer.ToString());
        }

        [TestMethod]
        public void ShouldParseHtmlAndProcess()
        {
            var writer = new StringWriter();
            var rendering = HTML.ToRendering();

            rendering.Process(writer);

            var actual = writer.ToString();
            Assert.AreEqual<string>(this.HTML, actual);
        }

        [TestMethod]
        public void ShouldSetDoublePingsOnAttributes()
        {
            var writer = new StringWriter();
            var rendering = "<input type=\"text\" value='content' />".ToRendering();

            rendering.Process(writer);
            Assert.AreEqual<string>("<input type=\"text\" value=\"content\" />", writer.ToString());
        }

        [TestMethod]
        public void ShouldParseTextAndHtmlMixed()
        {
            var rendering = "<div>This i some <i>mixed</i> text and html</div>".ToRendering();

            Assert.IsInstanceOfType(rendering[0], typeof(Literal)); // This is some
            Assert.IsInstanceOfType(rendering[1], typeof(HtmlTag)); // <i>
            Assert.IsInstanceOfType(rendering[1][0], typeof(Literal)); // mixed
            Assert.IsInstanceOfType(rendering[2], typeof(Literal)); // text and html
        }

        [TestMethod]
        public void ToRenderingShouldParseHtmlChunck()
        {
            var res = "<div></div>".ToRendering();

            Assert.IsInstanceOfType(res, typeof(HtmlTag));
            Assert.AreEqual("div", ((HtmlTag)res).Name);
            Assert.IsFalse(((HtmlTag)res).IsFullTag);
        }
    }
}