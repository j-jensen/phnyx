using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;
using Phnyx.Web;
using Phnyx.Web.Renderings;

namespace Phnyx.Web.Renderings.Test
{
    [TestClass]
    public class MapPathVisitorTest
    {
        [TestMethod]
        public void ShouldMakeRootpathAppPath()
        {
            var rendering = "<img src='/images/myimage.gif' />".ToRendering();

            var sut = new MapPathVisitor("/Products/Furniture/");
            sut.Visit(rendering);

            Assert.AreEqual("/images/myimage.gif", rendering.Attributes["src"]);
        }

        [TestMethod]
        public void ShouldFixRelativePath()
        {
            var rendering = "<img src='../images/myimage.gif' />".ToRendering();
            var appPath = "/Products/Furniture/";
            var templatePath = "/Layouts/template.html";

            var sut = new MapPathVisitor(appPath, templatePath);
            sut.Visit(rendering);

            Assert.AreEqual("/images/myimage.gif", rendering.Attributes["src"]);
        }

        [TestMethod]
        public void ShouldMakeAppRootpathAppPath()
        {
            var rendering = "<img src='../images/myimage.gif' />".ToRendering();

            var sut = new MapPathVisitor("/Products/Furniture/", "~/layout/index.htm");
            sut.Visit(rendering);

            Assert.AreEqual("/Products/Furniture/images/myimage.gif", rendering.Attributes["src"]);
        }
    }
}
