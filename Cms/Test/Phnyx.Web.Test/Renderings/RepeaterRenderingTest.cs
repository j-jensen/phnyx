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
    public class RepeaterRenderingTest
    {
        [TestMethod]
        public void ShouldTakeIEnumerableAsSourceAndRepeatWithFormatter()
        {
            var writer = new StringWriter();
            var sut = new Repeater<int>();
            sut.SetSource(Enumerable.Range(1, 10));
            sut.SetFormatter(x => string.Format("{0},", x));

            sut.Process(writer);

            Assert.AreEqual("1,2,3,4,5,6,7,8,9,10,", writer.ToString());
        }

        [TestMethod]
        public void ShouldRepeatWithoutFormatter()
        {
            var writer = new StringWriter();
            var sut = new Repeater<int>();
            sut.SetSource(Enumerable.Range(1, 10));

            sut.Process(writer);

            Assert.AreEqual("12345678910", writer.ToString());
        }

    }
}
