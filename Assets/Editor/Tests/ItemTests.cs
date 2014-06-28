using UnityEngine;
using System;
using System.Threading;
using NUnit.Framework;
using NSubstitute;
using System.Collections;
using System.Collections.Generic;

namespace UnityTest
{
    [TestFixture]
    internal class ItemTests
    {
        List<Industry> industries;
        List<ProductType> productTypes;
        List<Market> markets;
        private Item item;

        [SetUp]
        public void SetUp() {
            industries = new List<Industry>();
            industries.Add(new Industry("example_Industry"));
            productTypes = new List<ProductType>();
            markets = new List<Market>();
            item = new Item("example_Item", 500, industries, productTypes, markets);
        }

        [TearDown]
        public void TearDown() {
            industries = null;
            productTypes = null;
            markets = null;
            item = null;
        }

        [Test]
        public void ItemConstructor() {
            Assert.IsNotNull(item);
            Assert.AreEqual(item.name, "example_Item");
        }
    }
}