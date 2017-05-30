using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.IO;
using System.Linq;
using Phnyx.Web.Optimization;

namespace WebTest
{
    [TestClass]
    public class CombineFilterTest
    {
		byte[] testdata;

        public CombineFilterTest()
		{
            string file = Environment.GetEnvironmentVariable("TFS2012") + @"\DGI\Main\Source\OTS\EventPortal\WebLogicTest\scriptcombine.html";
			var r = File.OpenText(file);
			string html = r.ReadToEnd();
			r.Dispose();
			testdata = Encoding.UTF8.GetBytes(html);
		}

		[TestMethod]
		public void Write_CompleteBuffer_4Scripts()
		{
			var ms = new MemoryStream();
            var sut = new ScriptCombineFilter(ms);

			sut.Write(testdata, 0, testdata.Length);

			Assert.AreEqual(4, sut.FoundResources);
		}

		[TestMethod, Ignore]  //TODO: Problemet opstår, hvis en script-tag deles af bufferen. Kan give meget mystiske debug-scenarier
		public void Write_2xBuffer_4Scripts()
		{
			var ms = new MemoryStream();
            var sut = new ScriptCombineFilter(ms);
            int split = 53335;

			sut.Write(testdata, 0, split);
			sut.Write(testdata, split, testdata.Length-split);

			Assert.AreEqual(4, sut.FoundResources);
		}

        [TestMethod]
        public void Write_FindsBundle()
        {
			var ms = new MemoryStream();
            var sut = new ScriptCombineFilter(ms);

            sut.Write(testdata, 0, testdata.Length);

            var resultingHtml = Encoding.UTF8.GetString(ms.GetBuffer());
            Assert.IsTrue(resultingHtml.Contains("<script src=\"libs.js"));
        }

        [TestMethod,Ignore] // TODO: Skulle en inner-tag bruge data-bundle attributten, er vi på den...
        public void Write_OnlyFindsBundleInScriptTag()
        {
            var html = Encoding.UTF8.GetBytes("<script type='text/javascript><div data-bundle=\"somecrappyclass\">La la la la.</div></script>");
            var ms = new MemoryStream();
            var sut = new ScriptCombineFilter(ms);

            sut.Write(html, 0, html.Length);

            Assert.IsFalse(sut.Bundles.Contains("somecrappyclass"));
        }
    }
}
