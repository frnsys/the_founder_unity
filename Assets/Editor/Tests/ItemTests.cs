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
        private Item item;

        [SetUp]
        public void SetUp() {
            item = new Item("example_Item", 500);
        }

        [TearDown]
        public void TearDown() {
            item = null;
        }

        [Test]
        public void ItemConstructor() {
            Assert.IsNotNull(item);
            Assert.AreEqual(item.name, "example_Item");
        }
    }
}