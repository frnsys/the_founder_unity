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
	internal class CompanyTests
	{
        private Company c = null;
        private Worker worker = null;

        [SetUp]
        public void SetUp() {
            c = new Company("Foo Inc");
            worker = new Worker(0, 0, 0, 0, 0);
        }

        [TearDown]
        public void TearDown() {
            worker = null;
            c = null;
        }

		[Test]
		public void CompanyConstructor()
		{
            Assert.IsNotNull(c);

            // Creates a name.
            Assert.AreEqual(c.name, "Foo Inc");
		}

		[Test]
		public void ManageWorkers()
		{
            Assert.AreEqual(c.workers.Count, 0);

            c.sizeLimit = 0;
            c.HireWorker(worker);
            Assert.AreEqual(c.workers.Count, 0);

            c.sizeLimit = 10;
            c.HireWorker(worker);
            Assert.AreEqual(c.workers.Count, 1);

            c.FireWorker(worker);
            Assert.AreEqual(c.workers.Count, 0);
        }

		[Test]
		public void Pay()
		{
            worker.salary = 500;
            c.cash = 2000;
            c.HireWorker(worker);

            c.Pay();
            Assert.AreEqual(c.cash, 1500);
        }

		[Test]
		public void DevelopProduct()
		{
            c.HireWorker(worker);

            worker.productivity.baseValue = 10;
            worker.charisma.baseValue = 10;
            worker.creativity.baseValue = 10;
            worker.cleverness.baseValue = 10;

            IProduct p = Substitute.For<IProduct>();
            c.DevelopProduct(p);

            p.Received().Develop(10, 10, 10, 10);
        }

		[Test]
		public void Buy_CanAfford() {
            c.cash = 2000;
            List<Industry> industries = new List<Industry>();
            List<ProductType> productTypes = new List<ProductType>();
            List<Market> markets = new List<Market>();
            EquippableItem i = new EquippableItem("Foo", 500, industries, productTypes, markets);

            Assert.IsTrue(c.BuyItem(i));
            Assert.AreEqual(c.cash, 1500);
            Assert.AreEqual(c.items.Count, 1);
        }

		[Test]
		public void Buy_CannotAfford() {
            c.cash = 200;
            List<Industry> industries = new List<Industry>();
            List<ProductType> productTypes = new List<ProductType>();
            List<Market> markets = new List<Market>();
            EquippableItem i = new EquippableItem("Foo", 500, industries, productTypes, markets);

            Assert.IsFalse(c.BuyItem(i));
            Assert.AreEqual(c.cash, 200);
        }
    }
}
