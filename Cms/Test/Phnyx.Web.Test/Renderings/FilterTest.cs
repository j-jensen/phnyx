using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Phnyx.Web.Renderings.Test
{
    [TestClass]
    public class FilterTest
    {
        [TestMethod]
        public void ShouldPassOnIdMatch()
        {
            var rendering = "<div id='myId' />".ToRendering();

            var sut = Filter.ElementId("myId");

            Assert.IsTrue(sut.Pass(rendering));
        }

        [TestMethod]
        public void ShouldPassOnIdMatchIgnoreCase()
        {
            var rendering = "<div id='MYID' />".ToRendering();

            var sut = Filter.ElementId("myid");

            Assert.IsTrue(sut.Pass(rendering));
        }

        [TestMethod]
        public void ShouldNotPassOnIdMismatch()
        {
            var rendering = "<div id='notMyId' />".ToRendering();

            var sut = Filter.ElementId("myId");

            Assert.IsFalse(sut.Pass(rendering));
        }

        [TestMethod]
        public void ShouldPassMatchingElement()
        {
            var element = "<input name='yadiya'>".ToRendering();

            var sut = Filter.ElementName("input");

            Assert.IsTrue(sut.Pass(element));
        }

        [TestMethod]
        public void ShouldPassMatchingElementIgnoreCase()
        {
            var element = "<INPUT name='yadiya'>".ToRendering();

            var sut = Filter.ElementName("input");

            Assert.IsTrue(sut.Pass(element));
        }

        [TestMethod]
        public void ShouldNotPassMismatchingElement()
        {
            var element = "<br>".ToRendering();

            var sut = Filter.ElementName("input");

            Assert.IsFalse(sut.Pass(element));
        }

        [TestMethod]
        public void HasAttributeShouldMatchOnElementsWithAttribute()
        {
            var rendering = "<div custom='ca' /><div /><div name='custom' />".ToRendering();

            var sut = Filter.AttributeName("custom");

            Assert.IsTrue(sut.Pass(rendering[0]));
            Assert.IsFalse(sut.Pass(rendering[1]));
            Assert.IsFalse(sut.Pass(rendering[2]));
        }

        [TestMethod]
        public void OrShouldMatchOnElementsWithConcatedRestriction()
        {
            var rendering = "<div custom='ca' /><div /><div name='custom' />".ToRendering();

            var sut = Filter
                        .AttributeName("custom")
                        .Or(Filter.AttributeName("name"));

            Assert.IsTrue(sut.Pass(rendering[0]));
            Assert.IsFalse(sut.Pass(rendering[1]));
            Assert.IsTrue(sut.Pass(rendering[2]));
        }

        [TestMethod]
        public void AndShouldMatchOnElementsWithConcatedRestriction()
        {
            var rendering = "<div custom='ca' /><div /><div name='custom' />".ToRendering();

            var sut = Filter
                        .AttributeName("custom")
                        .And(Filter.ElementName("DIV"));

            Assert.IsTrue(sut.Pass(rendering[0]));
            Assert.IsFalse(sut.Pass(rendering[1]));
            Assert.IsFalse(sut.Pass(rendering[2]));
        }

        [TestMethod]
        public void AttributeShouldMatchOnElements()
        {
            var rendering = "<div name='ca' /><div /><div name='custom' />".ToRendering();

            var sut = Filter.Attribute("name", "ca");

            Assert.IsTrue(sut.Pass(rendering[0]));
            Assert.IsFalse(sut.Pass(rendering[1]));
            Assert.IsFalse(sut.Pass(rendering[2]));
        }

        [TestMethod]
        public void AttributeShouldNotMatchOnElementsMismatch()
        {
            var rendering = "<div name='ca' /><div /><div name='custom' />".ToRendering();

            var sut = Filter.Attribute("name", "xx");

            Assert.IsFalse(sut.Pass(rendering[0]));
            Assert.IsFalse(sut.Pass(rendering[1]));
            Assert.IsFalse(sut.Pass(rendering[2]));
        }

        [TestMethod]
        public void EmptyShouldMatchOnAllElements()
        {
            var rendering = "<div name='ca' /><div /><div name='custom' />".ToRendering();

            var sut = Filter.AlwaysTrue();

            Assert.IsTrue(sut.Pass(rendering[0]));
            Assert.IsTrue(sut.Pass(rendering[1]));
            Assert.IsTrue(sut.Pass(rendering[2]));
        }

        [TestMethod]
        public void ShouldPassOnCssClassMatch()
        {
            var rendering = "<div class='myClass' />".ToRendering();

            var sut = Filter.CssClass("myClass");

            Assert.IsTrue(sut.Pass(rendering));
        }

        [TestMethod]
        public void ShouldPassOnCssClassMatchIgnoreCase()
        {
            var rendering = "<div class='MYCLASS' />".ToRendering();

            var sut = Filter.CssClass("myClass");

            Assert.IsTrue(sut.Pass(rendering));
        }

        [TestMethod]
        public void ShouldNotPassOnCssClassMismatch()
        {
            var rendering = "<div class='notMyClass' />".ToRendering();

            var sut = Filter.CssClass("myClass");

            Assert.IsFalse(sut.Pass(rendering));
        }
    }
}
