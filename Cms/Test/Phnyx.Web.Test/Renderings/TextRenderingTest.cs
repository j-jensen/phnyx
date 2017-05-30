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
    public class TextRenderingTest
    {
        [TestMethod]
        public void ShouldHaveContent()
        {
            string content = "abcdefghijklmno";
            var sut = new Literal(content);
            Assert.AreEqual(content, sut.Content);
        }

        [TestMethod]
        public void ShouldProcessContentWithNoMarkup()
        {
            string content = "abcdefghijklmno";
            var sut = new Literal(content);
            var writer = new StringWriter();

            sut.Process(writer);

            Assert.AreEqual(content, writer.ToString());
        }
    }
}
