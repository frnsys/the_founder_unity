using UnityEngine;
using System;
using System.Threading;
using NUnit.Framework;

namespace UnityTest
{
	[TestFixture]
	internal class ProductTests
	{
        private Product p = null;

        [SetUp]
        public void SetUp() {
            ProductType pt = new ProductType("example_ProductType");
            Industry i = new Industry("example_Industry");
            Market m = new Market("example_Market");
            p = new Product(pt, i, m);
        }

        [TearDown]
        public void TearDown() {
            p = null;
        }

		[Test]
		public void ProductConstructor()
		{
            Assert.IsNotNull(p);

            // Creates a name.
            Assert.AreNotEqual(p.name, "");

            // Loads interaction.
            Assert.AreEqual(p.revenueModel, RevenueModel.GAUSSIAN);
            Assert.AreEqual(p.result, "Super Cool Product");
		}

		[Test]
		public void Develop()
		{
            Assert.AreEqual(p.progress, 0);
            Assert.AreEqual(p.appeal.value, 0);
            Assert.AreEqual(p.usability.value, 0);
            Assert.AreEqual(p.performance.value, 0);

            p.Develop(10, 5, 15, 25);

            Assert.AreEqual(p.progress, 10);
            Assert.AreEqual(p.appeal.value, 5);
            Assert.AreEqual(p.usability.value, 15);
            Assert.AreEqual(p.performance.value, 25);
            Assert.IsTrue(p.completed);
        }

		[Test]
		public void Revenue()
        {
            // Product must be complete first.
            p.Develop(10, 5, 15, 25);
            Assert.IsTrue(p.completed);

            Assert.IsTrue(p.Revenue(100) > 0);
        }
    }
}
