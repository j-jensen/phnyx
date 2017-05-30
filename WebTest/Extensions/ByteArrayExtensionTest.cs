using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Phnyx;

namespace Webtest
{
    [TestClass]
    public class ByteArrayExtensionTest
    {
        byte[] population = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        [TestMethod]
        public void Match_NoMatch_Comparable()
        {
            byte[] start = new byte[] { 2, 3 }, end = new byte[] { 77 };

            var m = population.Match(start, end);

            Assert.IsFalse(m.IsMatch);
        }

        [TestMethod]
        public void Match_StartIndexAndCount()
        {
            byte[] start = new byte[] { 2, 3 }, end = new byte[] { 7 };

            var m = population.Match(start, end);

            Assert.AreEqual(2, m.Begin);
            Assert.AreEqual(6, m.Length);
        }

        [TestMethod]
        public void Match_TightPatterns_StartIndexAndCount()
        {
            byte[] start = new byte[] { 2, 3 }, end = new byte[] { 4, 5 };

            var m = population.Match(start, end);

            Assert.AreEqual(2, m.Begin);
            Assert.AreEqual(4, m.Length);
        }

        [TestMethod]
        public void Match_ApartPatterns_StartIndexAndCount()
        {
            byte[] start = new byte[] { 2, 3 }, end = new byte[] { 5, 6, 7 };

            var m = population.Match(start, end);

            Assert.AreEqual(2, m.Begin);
            Assert.AreEqual(6, m.Length);
        }

        [TestMethod]
        public void Match_TightPatterns_StartIndexAndCount2()
        {
            byte[] start = new byte[] { 2, 3 }, end = new byte[] { 4, 5, 6 };

            var m = population.Match(start, end);

            Assert.AreEqual(2, m.Begin);
            Assert.AreEqual(5, m.Length);
        }

        [TestMethod]
        public void Match_ShouldNotLookForEndPatternInBeginPatternMatch()
        {
            byte[] start = new byte[] { 2, 3 }, end = new byte[] { 3 };

            var m = population.Match(start, end);

            Assert.IsFalse(m.IsMatch);
        }

        [TestMethod]
        public void Match_EndPatternLength1PatternMatch()
        {
            byte[] start = new byte[] { 2, 3 }, end = new byte[] { 6 };

            var m = population.Match(start, end);

            Assert.IsTrue(m.IsMatch);
            Assert.AreEqual(5, m.Length);
        }

        [TestMethod]
        public void SubArray_SlotAtStartindexShouldBeIncluded()
        {
            var expected = new byte[] { 8, 9, 10 };

            var actual = population.SubArray(8);

            Assert.AreEqual(expected[0], actual[0]);
            Assert.AreEqual(expected[1], actual[1]);
            Assert.AreEqual(expected[2], actual[2]);
        }

        [TestMethod]
        public void IndexOf_NoMatch_DueToCount()
        {
            var pos = population.IndexOfPattern(new byte[] { 6, 7 }, 0, 7);

            Assert.AreEqual(-1, pos);
        }

        [TestMethod]
        public void IndexOf_Match_DueToOffsetAndCount()
        {
            var pos = population.IndexOfPattern(new byte[] { 6, 7 }, 6, 2);

            Assert.AreEqual(6, pos);
        }

        [TestMethod]
        public void IndexOf_NoMatch_DueToOffsetAndCount()
        {
            var pos = population.IndexOfPattern(new byte[] { 6, 7 }, 5, 2);

            Assert.AreEqual(-1, pos);
        }

        [TestMethod]
        public void SubArray_WithLength()
        {
            var expected = new byte[] { 7, 8 };

            var actual = population.SubArray(7, 2);

            Assert.AreEqual(expected[0], actual[0]);
            Assert.AreEqual(expected[1], actual[1]);
        }

        [TestMethod]
        public void IndexOfPattern_ShouldReturn_StartIndex()
        {
            var pattern = new byte[] { 7, 8 };

            var pos = population.IndexOfPattern(pattern);

            Assert.AreEqual(7, pos);
        }

        [TestMethod]
        public void IndexOfPattern_ShouldNotMatcPartialEnd()
        {
            var pattern = new byte[] { 9, 10, 11, 12 };

            var pos = population.IndexOfPattern(pattern);

            Assert.IsFalse(pos > 0);
        }

        [TestMethod]
        public void IndexOfPattern_ShouldMatcSingleElement()
        {
            var pattern = new byte[] { 9 };

            var pos = population.IndexOfPattern(pattern);

            Assert.AreEqual(9, pos);
        }

        [TestMethod]
        public void Match_IsPartialTrueIfStartPatternMatch()
        {
            var pattern = new byte[] { 8, 9 };

            var m = population.Match(pattern, new byte[]{ 11 });

            Assert.IsTrue(m.IsPartial);
            Assert.AreEqual(m.Length, 2);
        }

        [TestMethod, Ignore] // TODO: Når en chunk splitter en tag
        public void Match_IsPartialTrueIfStartPatternPartialMatchEndEnd()
        {
            var pattern = new byte[] { 8, 9, 10, 11 };

            var m = population.Match(pattern, new byte[] {  });

            Assert.IsTrue(m.IsPartial);
            Assert.AreEqual(m.Length, 3);
        }
    }
}
