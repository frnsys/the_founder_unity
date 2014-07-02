using UnityEngine;
using UnityEditor;
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

        [SetUp]
        public void SetUp() {
            gameObj = new GameObject("Game Manager");
            gameObj.AddComponent<GameManager>();
            gameManager = gameObj.GetComponent<GameManager>();
            gameManager.LoadResources();

            c = new Company("Foo Inc");
            worker = new Worker(0, 0, 0, 0, 0);
            item = AssetDatabase.LoadAssetAtPath("Assets/Editor/Tests/Resources/TestItem.asset", typeof(Item)) as Item;
        }

        [TearDown]
        public void TearDown() {
            UnityEngine.Object.DestroyImmediate(gameObj);
            gameManager = null;
            worker = null;
            c = null;
            item = null;
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

            c.cash.baseValue = 2000;
            c.BuyItem(item);
            c.sizeLimit = 10;
            c.HireWorker(worker);
            Assert.AreEqual(c.workers.Count, 1);
            Assert.AreEqual(c.workers[0].happiness.value, 10);

            c.FireWorker(worker);
            Assert.AreEqual(c.workers.Count, 0);
        }

		[Test]
		public void Pay() {
            worker.salary = 500;
            c.cash.baseValue = 2000;
            c.HireWorker(worker);

            c.Pay();
            Assert.AreEqual(c.cash.baseValue, 1500);
        }

        [Test]
        public void StartNewProduct() {
            c.cash.baseValue = 2000;
            c.BuyItem(item);
            c.StartNewProduct();
            Assert.AreEqual(c.products.Count, 1);
            Assert.AreEqual(c.products[0].appeal.value, 10);
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
        public void RemoveProduct() {
            c.cash.baseValue = 2000;
            c.StartNewProduct();
            Product p = c.products[0];
            c.RemoveProduct(p);

            Assert.AreEqual(p.state, Product.State.RETIRED);
        }


		[Test]
		public void BuyItem_CanAfford() {
            c.cash.baseValue = 2000;
            c.StartNewProduct();
            Product p = c.products[0];
            c.HireWorker(worker);

            Assert.IsTrue(c.BuyItem(item));
            Assert.AreEqual(c.cash.baseValue, 1500);
            Assert.AreEqual(c.items.Count, 1);
            Assert.AreEqual(p.appeal.value, 10);
            Assert.AreEqual(worker.happiness.value, 10);
        }

		[Test]
		public void BuyItem_CannotAfford() {
            c.cash.baseValue = 200;
            Assert.IsFalse(c.BuyItem(item));
            Assert.AreEqual(c.cash.baseValue, 200);
        }

        [Test]
        public void RemoveItem() {
            c.cash.baseValue = 2000;
            c.StartNewProduct();
            Product p = c.products[0];
            c.HireWorker(worker);

            c.BuyItem(item);
            c.RemoveItem(item);

            Assert.AreEqual(c.items.Count, 0);
            Assert.AreEqual(p.appeal.value, 0);
            Assert.AreEqual(worker.happiness.value, 0);
        }
    }
}
