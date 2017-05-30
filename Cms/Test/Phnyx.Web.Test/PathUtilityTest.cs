using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Phnyx.Web.Test
{
    [TestClass]
    public class PathUtilityTest
    {
        [TestMethod]
        public void IsHostRoot_ShouldMatchRootUrl()
        {
            var url = "/Phnyx/doc.aspx";

            Assert.IsTrue(PathUtility.IsHostRoot(url));
        }

        [TestMethod]
        public void IsHostRoot_ShouldNotMatchUrl()
        {
            var url = "http://www.icoder.dk/Phnyx/doc.aspx";

            Assert.IsFalse(PathUtility.IsHostRoot(url));
        }

        [TestMethod]
        public void IsRelative_ShouldMatchUpdir()
        {
            var path = "../Phnyx/doc.aspx";

            Assert.IsTrue(PathUtility.IsRelativePath(path));
        }

        [TestMethod]
        public void Resolve_ShouldCombineScriptPathAndUpdirPath()
        {
            var path = "../Phnyx/doc.aspx";
            var scriptPath = "/Layouts/Shop/template.htm";

            Assert.AreEqual("/Layouts/Phnyx/doc.aspx", PathUtility.Resolve(path, "/", scriptPath));
        }

        [TestMethod]
        public void Resolve_ShouldNotCombineScriptPathAndRootedPath()
        {
            var path = "/Phnyx/doc.aspx";
            var scriptPath = "/Layouts/Shop/template.htm";

            Assert.AreEqual("/Phnyx/doc.aspx", PathUtility.Resolve(path, "/", scriptPath));
        }

        [TestMethod]
        public void Resolve_ShouldCombineScriptPathAndRelativePath()
        {
            var path = "Phnyx/doc.aspx";
            var scriptPath = "/Layouts/Shop/template.htm";

            Assert.AreEqual("/Layouts/Shop/Phnyx/doc.aspx", PathUtility.Resolve(path, "/", scriptPath));
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Resolve_ShouldThrowExceptionIfUpdirGoesOverRoot()
        {
            var path = "../doc.aspx";
            var scriptPath = "/template.htm";

            PathUtility.Resolve(path, "", scriptPath);
        }

        [TestMethod]
        public void GetDirectory_ShouldReturnTheDirectoryPartOfAUrl()
        {
            var url = "/MyApp/MyScript.aspx";

            Assert.AreEqual("/MyApp/", PathUtility.GetDirectory(url));
        }

        [TestMethod]
        public void GetDirectory_ShouldThrowExceptionIfNotRooted()
        {
            var url = "MyScript.aspx";
            object exc = null;
            try
            {
                PathUtility.GetDirectory(url);
            }
            catch(Exception e)
            {
                exc = e;
            }
            Assert.IsInstanceOfType(exc, typeof(ArgumentException));
        }

        [TestMethod]
        public void WalkUpdirs_ShouldRemoveDirsInLineAndMergeResult()
        {
            var result = PathUtility.WalkUpdirs("../../image.jpg", "/folder1/folder2/folder3/file.htm");

            Assert.AreEqual("/folder1/image.jpg", result);
        }

        [TestMethod]
        public void RemoveTrailingSlashes_ShouldRemoveFromEnd()
        {
            var result = PathUtility.RemoveTrailingSlashes("/test/");

            Assert.AreEqual("/test", result);
        }

        public void RemoveTrailingSlashes_ShouldNotRemoveFromEnd()
        {
            var result = PathUtility.RemoveTrailingSlashes("/test");

            Assert.AreEqual("/test", result);
        }
    }
}
