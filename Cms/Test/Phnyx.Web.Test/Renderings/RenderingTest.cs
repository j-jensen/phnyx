using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Phnyx.Web.Renderings;

namespace Phnyx.Web.Renderings.Test
{
    [TestClass]
    public class RenderingTest
    {
        [TestMethod]
        public void ShoudBeAbleToAddSubRenderings()
        {
            var sut = new FakeRendering();
            sut.Add(new Literal(""));
        }

        [TestMethod]
        public void CountShouldReflectTheNumberOfSubRenderings()
        {
            var sut = new FakeRendering();
            sut.Add(new Literal(""));
            sut.Add(new Literal(""));

            Assert.AreEqual(2, sut.Count);
        }

        [TestMethod]
        public void ClearShouldRemoveAllSubRenderings()
        {
            var sut = new FakeRendering();
            sut.Add(new Literal(""));
            sut.Add(new Literal(""));

            sut.Clear();

            Assert.AreEqual(0, sut.Count);
        }

        [TestMethod]
        public void SetAttributeShoudUpdateAttribute()
        {
            var sut = new FakeRendering();
            sut.Attributes["a1"] = "1";
            sut.Attributes["a1"] = "2";

            Assert.AreEqual("2", sut.Attributes["a1"]);
        }

        [TestMethod]
        public void ShouldAcceptVisitorOnChildrenAndLetItVisit()
        {
            var sut = new FakeRendering();
            sut.Add(new FakeRendering());
            sut.Accept(new FakeVisitor());

            Assert.IsTrue(((FakeRendering)sut[0]).IsVisited);
        }

        [TestMethod]
        public void VisitorShouldVisitFilteredChildren()
        {
            var sut = new FakeRendering();
            sut.Add(new FakeRendering());
            sut.Add(new FakeRendering());
            sut[0].Attributes["id"] = "node0";
            sut[1].Attributes["id"] = "node1";
            var visitor = new FakeVisitor();
            visitor.Filter = Filter.ElementId("node1");
            sut.Accept(visitor);

            Assert.IsFalse(((FakeRendering)sut[0]).IsVisited);
            Assert.IsTrue(((FakeRendering)sut[1]).IsVisited);
        }

        [TestMethod]
        public void VisitorShouldVisitAllChildrenRecursive()
        {
            var sut = new FakeRendering();
            sut.Add(new FakeRendering());
            sut[0].Add(new FakeRendering());

            sut.Accept(new FakeVisitor());

            Assert.IsTrue(((FakeRendering)sut[0]).IsVisited);
            Assert.IsTrue(((FakeRendering)sut[0][0]).IsVisited);
        }

        [TestMethod]
        public void FindShouldReturnFirstRenderingWithMatch()
        {
            var sut = "<div><p><div id='123'>Ya ya</div>ya ya ya</p></div>".ToRendering();

            var q = sut.Find(Filter.ElementId("123")) as HtmlTag;

            Assert.AreEqual("div", q.Name);
            Assert.IsInstanceOfType(q[0], typeof(Literal));
        }
    }

    // Only way to test an abstract class - Fake it!
    class FakeRendering : RenderingBase
    {
        public bool IsVisited = false;
    }

    class FakeVisitor : VisitorBase
    {
        public override void OnVisit(RenderingBase rendering)
        {
              ((FakeRendering)rendering).IsVisited = true;
        }
    }

}
