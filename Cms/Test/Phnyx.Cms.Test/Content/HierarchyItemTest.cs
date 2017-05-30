using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Phnyx.Cms.Content;

namespace Phnyx.Cms.Test.Content
{
    [TestClass]
    public class HierarchyItemTest
    {
        [TestMethod]
        public void ShouldHaveParentItemAndRessourceName()
        {
            HierarchyItem sut = new Item("Area 1", new Item("Root"));

            Assert.AreEqual("/Area%201", sut.RessourceName);
            Assert.IsNotNull(sut.Parent);
        }

        [TestMethod]
        public void RootItemShoudReturnSlashAsRessourceName()
        {
            HierarchyItem sut = new Item("Root");

            Assert.AreEqual("/", sut.RessourceName);
        }

        #region Impl HierarchyItem
        public class Item : HierarchyItem
        {
            public Item(string title)
                : this(title, null)
            {
            }

            public Item(string title, HierarchyItem parent) : base(title, parent) { }
        }
        #endregion
    }
}
