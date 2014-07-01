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
        private GameObject gameObj;
        private GameManager gameManager;
        private Company c;
        private Worker worker;
        private Item item;
        List<Industry> industries;
        List<ProductType> productTypes;
        List<Market> markets;

        [SetUp]
        public void SetUp() {
            gameObj = new GameObject("Game Manager");
            gameObj.AddComponent<GameManager>();
            gameManager = gameObj.GetComponent<GameManager>();
            gameManager.LoadResources();

            c = new Company("Foo Inc");
            worker = new Worker(0, 0, 0, 0, 0);
            industries = new List<Industry>();
            industries.Add(Industry.Space);
            productTypes = new List<ProductType>();
            markets = new List<Market>();
            item = new Item("example_Item", 500, industries, productTypes, markets);
        }

        [TearDown]
        public void TearDown() {
            UnityEngine.Object.DestroyImmediate(gameObj);
            gameManager = null;
            worker = null;
            c = null;
            item = null;
            industries = null;
            productTypes = null;
            markets = null;
        }

		[Test]
		public void CompanyConstructor() {
            Assert.IsNotNull(c);

            // Creates a name.
            Assert.AreEqual(c.name, "Foo Inc");
		}

		[Test]
		public void ManageWorkers() {
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
		public void Pay() {
            worker.salary = 500;
            c.cash = 2000;
            c.HireWorker(worker);

            c.Pay();
            Assert.AreEqual(c.cash, 1500);
        }

        [Test]
        public void StartNewProduct() {
            c.StartNewProduct();
            Assert.AreEqual(c.products.Count, 1);
        }

		[Test]
		public void DevelopProduct() {
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
		public void BuyItem_CanAfford() {
            c.cash = 2000;
            c.StartNewProduct();
            Product p = c.products[0];
            c.HireWorker(worker);

            Assert.IsTrue(c.BuyItem(item));
            Assert.AreEqual(c.cash, 1500);
            Assert.AreEqual(c.items.Count, 1);
            Assert.AreEqual(p.appeal.value, 10);
            Assert.AreEqual(worker.happiness.value, 10);
        }

		[Test]
		public void BuyItem_CannotAfford() {
            c.cash = 200;
            Assert.IsFalse(c.BuyItem(item));
            Assert.AreEqual(c.cash, 200);
        }

        [Test]
        public void RemoveItem() {
            c.cash = 2000;
            c.BuyItem(item);
            c.RemoveItem(item);

            Assert.AreEqual(c.items.Count, 0);
        }
    }
}
