using UnityEngine;
using UnityEditor;
using System;
using System.Threading;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

namespace UnityTest
{
	[TestFixture]
	internal class ProductTests
	{
        private Product p = null;
        private Item item;

        [SetUp]
        public void SetUp() {
            ProductType pt = ProductType.Social_Network;
            Industry i = Industry.Space;
            Market m = Market.Millenials;
            p = new Product(pt, i, m);

            item = AssetDatabase.LoadAssetAtPath("Assets/Editor/Tests/Resources/TestItem.asset", typeof(Item)) as Item;
        }

        [TearDown]
        public void TearDown() {
            p = null;
            item = null;
        }

		[Test]
		public void ProductConstructor() {
            Assert.IsNotNull(p);

            // Creates a name.
            Assert.AreNotEqual(p.name, "");
		}

		[Test]
		public void Develop() {
            Assert.AreEqual(p.state, Product.State.DEVELOPMENT);
            Assert.AreEqual(p.progress, 0);
            Assert.AreEqual(p.appeal.value, 0);
            Assert.AreEqual(p.usability.value, 0);
            Assert.AreEqual(p.performance.value, 0);

            p.Develop(1000, 5, 15, 25);

            Assert.AreEqual(p.progress, 1000);
            Assert.AreEqual(p.appeal.value, (5+15)/2);
            Assert.AreEqual(p.usability.value, (25+5)/2);
            Assert.AreEqual(p.performance.value, (15+25)/2);
            Assert.AreEqual(p.state, Product.State.LAUNCHED);
        }

		[Test]
		public void Revenue_NotLaunched() {
            Assert.AreNotEqual(p.state, Product.State.LAUNCHED);
            Assert.AreEqual(p.Revenue(10), 0);
        }

		[Test]
		public void Revenue_Launched() {
            p.appeal.baseValue = 100;
            p.usability.baseValue = 100;
            p.performance.baseValue = 100;

            p.Launch();

            Assert.IsTrue(p.Revenue(4) > 0);
        }

		[Test]
		public void Revenue_ZeroStats() {
            p.appeal.baseValue = 0;
            p.usability.baseValue = 0;
            p.performance.baseValue = 0;

            p.Launch();

            float zeroRev = p.Revenue(4);

            // You still make a little money.
            Assert.IsTrue(zeroRev > 0);


            // But it should be less than a product with more stats.
            p.appeal.baseValue = 100;
            p.usability.baseValue = 100;
            p.performance.baseValue = 100;

            p.Launch();

            Assert.IsTrue(p.Revenue(4) > zeroRev);
        }

        [Test]
        public void ApplyItem() {
            p.ApplyItem(item);
            Assert.AreEqual(p.appeal.value, 10);
        }

		[Test]
		public void Shutdown() {
            p.Shutdown();

            Assert.AreEqual(p.state, Product.State.RETIRED);
        }
    }
}
