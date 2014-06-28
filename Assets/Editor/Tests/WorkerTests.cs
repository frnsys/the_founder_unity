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
    internal class WorkerTests
    {
        private Worker worker;
        List<Industry> industries;
        List<ProductType> productTypes;
        List<Market> markets;
        private Item item;

        [SetUp]
        public void SetUp() {
            worker = new Worker(0, 0, 0, 0, 0);
            industries = new List<Industry>();
            industries.Add(new Industry("example_Industry"));
            productTypes = new List<ProductType>();
            markets = new List<Market>();
            item = new Item("example_Item", 500, industries, productTypes, markets);
        }
        
        [TearDown]
        public void TearDown() {
            worker = null;
        }

        [Test]
        public void WorkerConstructor() {
            Assert.IsNotNull(worker);
            Assert.AreEqual(worker.happiness.value, 0);
        }

        [Test]
        public void ApplyItem() {
            worker.ApplyItem(item);
            Assert.AreEqual(worker.happiness.value, 10);
        }
    }
}