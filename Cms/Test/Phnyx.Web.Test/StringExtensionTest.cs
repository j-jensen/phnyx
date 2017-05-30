using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Phnyx.Web.Test
{
    [TestClass]
    public class StringExtensionTest
    {

        [TestMethod]
        public void ShouldReturnTrueIfEqualCaseInsencitive()
        {
            string str1 = "Hej med Dig!";
            string str2 = "hej med dig!";

            Assert.IsTrue(str1.EqualsIgnoreCase(str2));
        }
    }
}
