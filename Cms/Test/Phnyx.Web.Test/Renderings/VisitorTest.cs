using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Phnyx.Web.Renderings;

namespace Phnyx.Web.Renderings.Test
{
    [TestClass]
    public class VisitorTest
    {
        [TestMethod]
        public void ShouldHaveFilter()
        {
            var sut = new FakeRenderingVisitor();
            sut.Filter =Filter.ElementId("xx");
        }
    }

    class FakeRenderingVisitor : VisitorBase
    {
        public override void OnVisit(RenderingBase rendering)
        {
            throw new NotImplementedException();
        }
    }
}
